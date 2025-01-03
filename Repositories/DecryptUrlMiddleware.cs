using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using System.Text;
using PurchasingSystemDeveloper.Repositories;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http.Extensions;

public class DecryptUrlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDataProtector _protector;
    private readonly UrlMappingService _urlMappingService;

    public DecryptUrlMiddleware(RequestDelegate next, IDataProtectionProvider provider, UrlMappingService urlMappingService)
    {
        _next = next;
        _protector = provider.CreateProtector("UrlProtector");
        _urlMappingService = urlMappingService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value.Trim('/');

        // Jika path kosong atau "undefined", fallback
        if (string.IsNullOrEmpty(path) || path.Equals("undefined", StringComparison.OrdinalIgnoreCase))
        {
            var currentUrl = context.Request.GetDisplayUrl(); // URL penuh saat ini

            if (!currentUrl.Contains("retry=1"))
            {
                // Tambah query param retry=1 untuk mencegah loop
                var separator = currentUrl.Contains("?") ? "&" : "?";
                context.Response.Redirect(currentUrl + separator + "retry=1");
            }
            else
            {
                // Sudah mencoba sekali, fallback ke halaman lain agar tidak loop selamanya
                context.Response.Redirect("/Account/Login");
            }
            return;
        }

        // Redirect '/Account/Login' ke URL terenkripsi
        if (string.Equals(path, "Account/Login", StringComparison.OrdinalIgnoreCase))
        {
            string originalPath = "Page:Account/Login";

            try
            {
                // Enkripsi path asli
                var encryptedPath1 = _protector.Protect(originalPath);

                // GUID-like code
                string guidLikeCode = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(encryptedPath1)))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Substring(0, 36);

                // Simpan mapping
                _urlMappingService.InMemoryMapping[guidLikeCode] = encryptedPath1;

                // Redirect ke URL terenkripsi
                context.Response.Redirect("/" + guidLikeCode);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating encrypted URL: {ex.Message}");
                RedirectToFallback(context);
                return;
            }
        }

        // Periksa apakah path adalah GUID-like code
        if (!string.IsNullOrEmpty(path) && path.Length == 36 && _urlMappingService.InMemoryMapping.TryGetValue(path, out var encryptedPath))
        {
            try
            {
                // Dekripsi path asli
                var decryptedPath = _protector.Unprotect(encryptedPath);

                // Periksa prefix untuk menentukan jenis URL
                if (decryptedPath.StartsWith("Create:"))
                {
                    // URL untuk detail produk
                    var actualPath = decryptedPath.Substring(7); // Hilangkan prefix "Detail:"
                    context.Request.Path = new PathString("/" + actualPath);
                }
                else if (decryptedPath.StartsWith("Detail:"))
                {
                    // URL untuk detail produk
                    var actualPath = decryptedPath.Substring(7); // Hilangkan prefix "Detail:"
                    var parts = actualPath.Split('/');
                    if (parts.Length == 4)
                    {
                        context.Request.Path = new PathString("/" + string.Join("/", parts.Take(3)));
                        context.Request.QueryString = new QueryString($"?id={parts[3]}");
                    }
                }
                else if (decryptedPath.StartsWith("Delete:"))
                {
                    // URL untuk detail produk
                    var actualPath = decryptedPath.Substring(7); // Hilangkan prefix "Detail:"
                    var parts = actualPath.Split('/');
                    if (parts.Length == 4)
                    {
                        context.Request.Path = new PathString("/" + string.Join("/", parts.Take(3)));
                        context.Request.QueryString = new QueryString($"?id={parts[3]}");
                    }
                }
                else if (decryptedPath.StartsWith("Page:"))
                {
                    // URL untuk paginasi
                    var actualPath = decryptedPath.Substring(5); // Hilangkan prefix "Page:"
                    var parts = actualPath.Split('?');
                    if (parts.Length > 0)
                    {
                        context.Request.Path = new PathString("/" + parts[0]);
                        if (parts.Length > 1)
                        {
                            context.Request.QueryString = new QueryString("?" + parts[1]);
                        }
                    }
                }
                else
                {
                    // Jika prefix tidak dikenali, fallback
                    RedirectToFallback(context);
                    return;
                }
            }
            catch
            {
                // Jika tidak ditemukan mapping, fallback
                RedirectToFallback(context);
                return;
            }
        }

        await _next(context);
    }

    private void RedirectToFallback(HttpContext context)
    {
        try
        {
            // Fallback original path
            string fallbackOriginalPath = "Page:Account/Login";
            var fallbackEncryptedPath = _protector.Protect(fallbackOriginalPath);
            string fallbackGuid = ConvertToGuidLikeCode(fallbackEncryptedPath);

            _urlMappingService.InMemoryMapping[fallbackGuid] = fallbackEncryptedPath;
            context.Response.Redirect("/" + fallbackGuid);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating fallback URL: {ex.Message}");
            // Jika bahkan membuat fallback gagal, terakhir: direct redirect (daripada loop)
            context.Response.Redirect("/Account/Login");
        }
    }

    private string ConvertToGuidLikeCode(string encryptedPath)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(encryptedPath)))
            .Replace('+', '-')
            .Replace('/', '_')
            .Substring(0, 36);
    }
}