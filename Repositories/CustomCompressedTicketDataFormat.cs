using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using System.IO.Compression;

public class CustomCompressedTicketDataFormat : ISecureDataFormat<AuthenticationTicket>
{
    private readonly ISecureDataFormat<AuthenticationTicket> _innerFormat;

    public CustomCompressedTicketDataFormat(IDataProtector dataProtector)
    {
        _innerFormat = new TicketDataFormat(dataProtector);
    }

    public string Protect(AuthenticationTicket data)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                var uncompressedData = System.Text.Encoding.UTF8.GetBytes(_innerFormat.Protect(data));
                gzipStream.Write(uncompressedData, 0, uncompressedData.Length);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }

    public string Protect(AuthenticationTicket data, string purpose)
    {
        return Protect(data);
    }

    public AuthenticationTicket Unprotect(string protectedText)
    {
        var compressedData = Convert.FromBase64String(protectedText);
        using (var memoryStream = new MemoryStream(compressedData))
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzipStream))
            {
                return _innerFormat.Unprotect(reader.ReadToEnd());
            }
        }
    }

    public AuthenticationTicket Unprotect(string protectedText, string purpose)
    {
        return Unprotect(protectedText);
    }
}
