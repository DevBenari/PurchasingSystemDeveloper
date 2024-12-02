using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;

        public DashboardController(
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService
        )
        {
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
        }

        public IActionResult RedirectToIndex()
        {
            try
            {
                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:MasterData/Dashboard/Index";
                string encryptedPath = _protector.Protect(originalPath);

                // Hash GUID-like code (SHA256 truncated to 36 characters)
                string guidLikeCode = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(encryptedPath)))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Substring(0, 36);

                // Simpan mapping GUID-like code ke encryptedPath di penyimpanan sementara (misalnya, cache)
                _urlMappingService.InMemoryMapping[guidLikeCode] = encryptedPath;

                return Redirect("/" + guidLikeCode);
            }
            catch
            {
                // Jika enkripsi gagal, kembalikan view
                return View();
            }            
        }

        public IActionResult Index()
        {
            ViewBag.Active = "MasterData";
            var countUser = _applicationDbContext.UserActives.GroupBy(u => u.UserActiveId).Select(y => new
            {
                UserActiveId = y.Key,
                CountOfUsers = y.Count()
            }).ToList();
            ViewBag.CountUser = countUser.Count;

            var countSupplier = _applicationDbContext.Suppliers.GroupBy(u => u.SupplierId).Select(y => new
            {
                SupplierId = y.Key,
                CountOfSuppliers = y.Count()
            }).ToList();
            ViewBag.CountSupplier = countSupplier.Count;

            var countUnitLocation = _applicationDbContext.UnitLocations.GroupBy(u => u.UnitLocationId).Select(y => new
            {
                UnitLocationId = y.Key,
                CountOfUnitLocations = y.Count()
            }).ToList();
            ViewBag.CountUnitLocation = countUnitLocation.Count;

            var countWarehouseLocation = _applicationDbContext.WarehouseLocations.GroupBy(u => u.WarehouseLocationId).Select(y => new
            {
                WarehouseLocationId = y.Key,
                CountOfWarehouseLocations = y.Count()
            }).ToList();
            ViewBag.CountWarehouseLocation = countWarehouseLocation.Count;

            var countProduct = _applicationDbContext.Products.GroupBy(u => u.ProductId).Select(y => new
            {
                ProductId = y.Key,
                CountOfProducts = y.Count()
            }).ToList();                        
            ViewBag.CountProduct = countProduct.Count;

            var countCategory = _applicationDbContext.Categories.GroupBy(u => u.CategoryId).Select(y => new
            {
                CategoryId = y.Key,
                CountOfCategories = y.Count()
            }).ToList();
            ViewBag.CountCategory = countCategory.Count;

            var countMeasurement = _applicationDbContext.Measurements.GroupBy(u => u.MeasurementId).Select(y => new
            {
                MeasurementId = y.Key,
                CountOfMeasurements = y.Count()
            }).ToList();
            ViewBag.CountMeasurement = countMeasurement.Count;

            var countDiscount = _applicationDbContext.Discounts.GroupBy(u => u.DiscountId).Select(y => new
            {
                DiscountId = y.Key,
                CountOfDiscounts = y.Count()
            }).ToList();
            ViewBag.CountDiscount = countDiscount.Count;

            return View();
        }
    }
}
