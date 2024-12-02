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
    public class ApiController : Controller
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

        public ApiController(
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

        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:Api/Api/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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
                return Redirect(Request.Path);
            }
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return BadRequest(new { message = "User sedang online || Response Code: 400" }); // 400 Bad Request
                }
                else
                {
                    // Cek apakah admin atau user lain
                    if (model.Email == "superadmin@admin.com" && model.Password == "Admin@123")
                    {
                        // Membuat token JWT
                        var jwtSettings = _configuration.GetSection("Jwt");
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var claims = new[]
                        {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                        var token = new JwtSecurityToken(
                            issuer: jwtSettings["Issuer"],
                            audience: jwtSettings["Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
                            signingCredentials: credentials
                        );

                        return RedirectToAction("RedirectToIndex", "Api");
                    }
                    else
                    {
                        var user = await _signInManager.UserManager.FindByNameAsync(model.Email);

                        // Cek apakah user ada
                        if (user == null)
                        {
                            return NotFound(new { message = "User belum terdaftar" });
                        }
                        else if (user.IsActive && !user.IsOnline)
                        {
                            // Cek password
                            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
                            if (result.Succeeded)
                            {
                                // Membuat token JWT
                                var jwtSettings = _configuration.GetSection("Jwt");
                                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
                                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                                var claims = new[]
                                {
                            new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                                var token = new JwtSecurityToken(
                                    issuer: jwtSettings["Issuer"],
                                    audience: jwtSettings["Audience"],
                                    claims: claims,
                                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
                                    signingCredentials: credentials
                                );

                                return RedirectToAction("RedirectToIndex", "Api");
                            }
                            else if (result.IsLockedOut)
                            {
                                var lockTime = await _userManager.GetLockoutEndDateAsync(user);
                                var timeRemaining = lockTime.Value - DateTimeOffset.UtcNow;

                                return BadRequest(new { message = "Akun Anda diblokir sementara" });
                            }
                            else
                            {
                                return Unauthorized(new { message = "Password salah" });
                            }
                        }
                        else if (user.IsActive && user.IsOnline)
                        {
                            // Jika user sudah login sebelumnya
                            user.IsOnline = false;
                            await _userManager.UpdateAsync(user);

                            return Ok(new { message = "Akun berhasil logout" });
                        }
                        else
                        {
                            return BadRequest(new { message = "Akun Anda belum aktif" });
                        }
                    }
                }
            }
            return View(model);  // Jika model tidak valid, kembali ke form login
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            ViewBag.Active = "MasterData";
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedFilter = filterOptions;

            // Format tanggal untuk input[type="date"]
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // Format tanggal untuk tampilan (Indonesia)
            ViewBag.StartDateReadable = startDate?.ToString("dd MMMM yyyy");
            ViewBag.EndDateReadable = endDate?.ToString("dd MMMM yyyy");

            // Normalisasi tanggal untuk mengabaikan waktu
            if (startDate.HasValue) startDate = startDate.Value.Date;
            if (endDate.HasValue) endDate = endDate.Value.Date.AddDays(1).AddTicks(-1); // Sampai akhir hari

            // Tentukan range tanggal berdasarkan filterOptions
            if (!string.IsNullOrEmpty(filterOptions))
            {
                (startDate, endDate) = GetDateRangeHelper.GetDateRange(filterOptions);
            }

            var data = await _productRepository.GetAllProductPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<Product>
            {
                Items = data.products,
                TotalCount = data.totalCountProducts,
                PageSize = pageSize,
                CurrentPage = page,
            };

            // Sertakan semua parameter untuk pagination
            ViewBag.FilterOptions = filterOptions;
            ViewBag.StartDateParam = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDateParam = endDate?.ToString("yyyy-MM-dd");
            ViewBag.PageSize = pageSize;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _productRepository.GetAllProduct().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.ProductCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.ProductCode = "PDC" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.ProductCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.ProductCode = "PDC" + setDateNow + "0001";
                }
                else
                {
                    vm.ProductCode = "PDC" + setDateNow + (Convert.ToInt32(lastCode.ProductCode.Substring(9, lastCode.ProductCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var Product = new Product
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    ProductId = vm.ProductId,
                    ProductCode = vm.ProductCode,
                    ProductName = vm.ProductName,
                    SupplierId = vm.SupplierId,
                    CategoryId = vm.CategoryId,
                    MeasurementId = vm.MeasurementId,
                    DiscountId = vm.DiscountId,
                    WarehouseLocationId = vm.WarehouseLocationId,
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

                var checkDuplicate = _productRepository.GetAllProduct().Where(c => c.ProductName == vm.ProductName).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _productRepository.GetAllProduct().Where(c => c.ProductName == vm.ProductName).FirstOrDefault();
                    if (result == null)
                    {
                        _productRepository.Tambah(Product);
                        TempData["SuccessMessage"] = "Name " + vm.ProductName + " Saved";
                        return RedirectToAction("Index", "Product");
                    }
                    else
                    {
                        ViewBag.Supplier = new SelectList(await _SupplierRepository.GetSuppliers(), "SupplierId", "SupplierName", SortOrder.Ascending);
                        ViewBag.Category = new SelectList(await _categoryRepository.GetCategories(), "CategoryId", "CategoryName", SortOrder.Ascending);
                        ViewBag.Measurement = new SelectList(await _measurementRepository.GetMeasurements(), "MeasurementId", "MeasurementName", SortOrder.Ascending);
                        ViewBag.Discount = new SelectList(await _discountRepository.GetDiscounts(), "DiscountId", "DiscountValue", SortOrder.Ascending);
                        ViewBag.Warehouse = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);

                        TempData["WarningMessage"] = "Name " + vm.ProductName + " Already Exist !!!";
                        return View(vm);
                    }
                }
                else
                {
                    ViewBag.Supplier = new SelectList(await _SupplierRepository.GetSuppliers(), "SupplierId", "SupplierName", SortOrder.Ascending);
                    ViewBag.Category = new SelectList(await _categoryRepository.GetCategories(), "CategoryId", "CategoryName", SortOrder.Ascending);
                    ViewBag.Measurement = new SelectList(await _measurementRepository.GetMeasurements(), "MeasurementId", "MeasurementName", SortOrder.Ascending);
                    ViewBag.Discount = new SelectList(await _discountRepository.GetDiscounts(), "DiscountId", "DiscountValue", SortOrder.Ascending);
                    ViewBag.Warehouse = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);

                    TempData["WarningMessage"] = "Name " + vm.ProductName + " There is duplicate data !!!";
                    return View(vm);
                }
            }
            else
            {
                ViewBag.Supplier = new SelectList(await _SupplierRepository.GetSuppliers(), "SupplierId", "SupplierName", SortOrder.Ascending);
                ViewBag.Category = new SelectList(await _categoryRepository.GetCategories(), "CategoryId", "CategoryName", SortOrder.Ascending);
                ViewBag.Measurement = new SelectList(await _measurementRepository.GetMeasurements(), "MeasurementId", "MeasurementName", SortOrder.Ascending);
                ViewBag.Discount = new SelectList(await _discountRepository.GetDiscounts(), "DiscountId", "DiscountValue", SortOrder.Ascending);
                ViewBag.Warehouse = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);

                return View(vm);
            }
        }

    }
}
