using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using PurchasingSystemDeveloper.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace PurchasingSystemDeveloper.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("Api/[Controller]/[Action]")]
    public class ProductApiController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMeasurementRepository _measurementRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ISupplierRepository _SupplierRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;


        private readonly IConfiguration _configuration;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;

        public ProductApiController(
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ICategoryRepository CategoryRepository,
            IMeasurementRepository MeasurementRepository,
            IDiscountRepository DiscountRepository,
            ISupplierRepository SupplierRepository,
            HttpClient httpClient,
            IWarehouseLocationRepository warehouseLocationRepository,

            IConfiguration configuration,

            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService
        )
        {
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;
            _httpClient = httpClient;
            _categoryRepository = CategoryRepository;
            _productRepository = productRepository;
            _measurementRepository = MeasurementRepository;
            _discountRepository = DiscountRepository;
            _SupplierRepository = SupplierRepository;
            _warehouseLocationRepository = warehouseLocationRepository;

            _configuration = configuration;

            _userManager = userManager;
            _signInManager = signInManager;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
        }


        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        // Get All Products
        [HttpGet("Products")]
        [AllowAnonymous]
        public IActionResult GetProduct()
        {
            var data = _productRepository.GetAllProduct().ToList();  // Materialize the result to a List
            return Ok(data);  // Return data as JSON
        }

        // Get All Categories
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCategories()
        {
            var data = _categoryRepository.GetAllCategory();  // Materialize the result to a List
            return Ok(data);  // Return data as JSON
        }

        // Get All Measurements
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetMeasurements()
        {
            var data = _measurementRepository.GetAllMeasurement();  // Materialize the result to a List
            return Ok(data);  // Return data as JSON
        }

        // Get All Discounts
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetDiscounts()
        {
            var data = _discountRepository.GetAllDiscount();  // Materialize the result to a List
            return Ok(data);  // Return data as JSON
        }

        // Get All Suppliers
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetSuppliers()
        {
            var data = _SupplierRepository.GetAllSupplier();  // Materialize the result to a List
            return Ok(data);  // Return data as JSON
        }

        // Get All Warehouse Locations
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetWarehouseLocations()
        {
            var data = _warehouseLocationRepository.GetAllWarehouseLocation().ToList();  // Materialize the result to a List
            return Ok(data);  // Return data as JSON
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateProduct([FromBody] ProductViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = dateNow.ToString("yyMMdd");

            // Mendapatkan kode produk terakhir yang dibuat pada hari yang sama
            var lastCode = _productRepository.GetAllProduct()
                .Where(d => d.CreateDateTime.ToString("yyMMdd") == setDateNow)
                .OrderByDescending(k => k.ProductCode).FirstOrDefault();

            string ProductCode;

            if (lastCode == null)
            {
                ProductCode = "PDC" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.ProductCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    ProductCode = "PDC" + setDateNow + "0001";
                }
                else
                {
                    ProductCode = "PDC" + setDateNow +
                                    (Convert.ToInt32(lastCode.ProductCode.Substring(9, lastCode.ProductCode.Length - 9)) + 1)
                                    .ToString("D4");
                }
            }

            // Mendapatkan user yang sedang login
            var getUser = _userActiveRepository.GetAllUserLogin()
                .FirstOrDefault(u => u.UserName == "barra@gmail.com");

            if (getUser == null)
            {
                return Unauthorized("User not found or unauthorized.");
            }

            // Pencarian entitas berdasarkan nama yang diterima
            var GetSupplier = _SupplierRepository.GetAllSupplier()
                .FirstOrDefault(c => c.SupplierName == vm.SupplierName);

            if (GetSupplier == null)
            {
                return BadRequest("Supplier not found.");
            }

            var GetCategory = _categoryRepository.GetAllCategory()
                .FirstOrDefault(c => c.CategoryName == vm.CategoryName);

            if (GetCategory == null)
            {
                return BadRequest("Category not found.");
            }

            var GetMeasurement = _measurementRepository.GetAllMeasurement()
                .FirstOrDefault(c => c.MeasurementName == vm.MeasurementName);

            if (GetMeasurement == null)
            {
                return BadRequest("Measurement not found.");
            }

            var GetDiscount = _discountRepository.GetAllDiscount()
                .FirstOrDefault(c => c.DiscountValue == vm.DiscountValue);

            if (GetDiscount == null)
            {
                return BadRequest("Discount not found.");
            }

            var GetWarehouseLocation = _warehouseLocationRepository.GetAllWarehouseLocation()
                .FirstOrDefault(c => c.WarehouseLocationName == vm.WarehouseLocationName);

            if (GetWarehouseLocation == null)
            {
                return BadRequest("Warehouse Location not found.");
            }

            // Membuat produk baru
            var product = new Product
            {
                CreateDateTime = DateTime.Now,
                CreateBy = new Guid(getUser.Id),
                ProductId = Guid.NewGuid(),
                ProductCode = ProductCode,
                ProductName = vm.ProductName,
                SupplierId = GetSupplier.SupplierId,
                CategoryId = GetCategory.CategoryId,
                MeasurementId = GetMeasurement.MeasurementId,
                DiscountId = GetDiscount.DiscountId,
                WarehouseLocationId = GetWarehouseLocation.WarehouseLocationId,
                MinStock = vm.MinStock,
                MaxStock = vm.MaxStock,
                BufferStock = vm.BufferStock,
                Stock = vm.Stock,
                Cogs = vm.Cogs,
                BuyPrice = vm.BuyPrice,
                RetailPrice = vm.RetailPrice,
                StorageLocation = vm.StorageLocation,
                RackNumber = vm.RackNumber,
                Note = vm.Note
            };

            // Simpan produk baru
           var result = _productRepository.Tambah(product);

            return Ok(new { Message = "Product created successfully!", result = result });
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductViewModel vm)
        {
            // Mencari produk yang akan diedit berdasarkan ProductId
            var product = _productRepository.GetAllProduct()
                                                   .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found!";
                return NotFound("Product not found.");
            }

            var getUser = _userActiveRepository.GetAllUserLogin()
                                               .FirstOrDefault(u => u.UserName == "barra@gmail.com");

            // Pencarian entitas berdasarkan nama yang diterima
            var GetSupplier = _SupplierRepository.GetAllSupplier()
                .FirstOrDefault(c => c.SupplierName == vm.SupplierName);

            if (GetSupplier == null)
            {
                return BadRequest("Supplier not found.");
            }

            var GetCategory = _categoryRepository.GetAllCategory()
                .FirstOrDefault(c => c.CategoryName == vm.CategoryName);

            if (GetCategory == null)
            {
                return BadRequest("Category not found.");
            }

            var GetMeasurement = _measurementRepository.GetAllMeasurement()
                .FirstOrDefault(c => c.MeasurementName == vm.MeasurementName);

            if (GetMeasurement == null)
            {
                return BadRequest("Measurement not found.");
            }

            var GetDiscount = _discountRepository.GetAllDiscount()
                .FirstOrDefault(c => c.DiscountValue == vm.DiscountValue);

            if (GetDiscount == null)
            {
                return BadRequest("Discount not found.");
            }

            var GetWarehouseLocation = _warehouseLocationRepository.GetAllWarehouseLocation()
                .FirstOrDefault(c => c.WarehouseLocationName == vm.WarehouseLocationName);

            if (GetWarehouseLocation == null)
            {
                return BadRequest("Warehouse Location not found.");
            }
            // Memperbarui data produk dengan data baru dari vm (ProductViewModel)
            product.ProductName = vm.ProductName;
            product.SupplierId = GetSupplier.SupplierId;
            product.CategoryId = GetCategory.CategoryId;
            product.MeasurementId = GetMeasurement.MeasurementId;
            product.DiscountId = GetDiscount.DiscountId;
            product.WarehouseLocationId = GetWarehouseLocation.WarehouseLocationId;
            product.MinStock = vm.MinStock;
            product.MaxStock = vm.MaxStock;
            product.BufferStock = vm.BufferStock;
            product.Stock = vm.Stock;
            product.Cogs = vm.Cogs;
            product.BuyPrice = vm.BuyPrice;
            product.RetailPrice = vm.RetailPrice;
            product.StorageLocation = vm.StorageLocation;
            product.RackNumber = vm.RackNumber;
            product.Note = vm.Note;
            product.UpdateDateTime = DateTime.Now;  // Waktu update
            product.UpdateBy = new Guid(getUser.Id);  // Update by user yang login

            // Mengecek apakah ada produk dengan nama yang sama (opsional, tergantung kebutuhan)
            var existingProduct = _productRepository.GetAllProduct()
                                                          .FirstOrDefault(c => c.ProductName == vm.ProductName && c.ProductId != id);

            if (existingProduct != null)
            {
                TempData["WarningMessage"] = $"Name '{vm.ProductName}' already exists!";
                return BadRequest("Product with the same name already exists.");
            }

            // Menyimpan perubahan ke repository
            _productRepository.Update(product);  // Pastikan ada method UpdateAsync di repository Anda

            TempData["SuccessMessage"] = $"Product '{vm.ProductName}' updated successfully.";
            return Ok(product);  // Mengembalikan objek produk yang telah diperbarui
        }


        [HttpDelete("{id}")]
        [AllowAnonymous]
        public IActionResult DeleteProduct(Guid id)
        {
            // Cari data berdasarkan ID
            var ResepMasuk = _applicationDbContext.Products.Find(id);
            if (ResepMasuk == null)
            {
                return NotFound($"ResepMasuk dengan ID {id} tidak ditemukan.");
            }

            try
            {
                // Hapus entitas dari database
                _applicationDbContext.Products.Remove(ResepMasuk);

                // Simpan perubahan
                _applicationDbContext.SaveChanges();

                return Ok(new { success = true, message = $"Berhasil menghapus product dengan id {id}" });
            }
            catch (Exception ex)
            {
                // Tangani error jika ada masalah
                return StatusCode(500, $"Terjadi kesalahan saat menghapus data: {ex.Message}");
            }
        }
    }
}