using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class WarehouseLocationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly IProductRepository _productRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public WarehouseLocationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IWarehouseLocationRepository warehouseLocationRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _warehouseLocationRepository = warehouseLocationRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:MasterData/WarehouseLocation/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

        [HttpGet]
        public async Task<IActionResult> Index(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
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

            var data = await _warehouseLocationRepository.GetAllWarehouseLocationPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<WarehouseLocation>
            {
                Items = data.warehouseLocations,
                TotalCount = data.totalCountWarehouseLocations,
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

        public IActionResult RedirectToCreate()
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Create:MasterData/WarehouseLocation/CreateWarehouseLocation";
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

        [HttpGet]
        public async Task<ViewResult> CreateWarehouseLocation()
        {
            //ViewBag.Active = "MasterData";

            ViewBag.WarehouseManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);

            var user = new WarehouseLocationViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _warehouseLocationRepository.GetAllWarehouseLocation().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.WarehouseLocationCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.WarehouseLocationCode = "WRH" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.WarehouseLocationCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.WarehouseLocationCode = "WRH" + setDateNow + "0001";
                }
                else
                {
                    user.WarehouseLocationCode = "WRH" + setDateNow + (Convert.ToInt32(lastCode.WarehouseLocationCode.Substring(9, lastCode.WarehouseLocationCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouseLocation(WarehouseLocationViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _warehouseLocationRepository.GetAllWarehouseLocation().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.WarehouseLocationCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.WarehouseLocationCode = "WRH" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.WarehouseLocationCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.WarehouseLocationCode = "WRH" + setDateNow + "0001";
                }
                else
                {
                    ViewBag.WarehouseManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                    vm.WarehouseLocationCode = "WRH" + setDateNow + (Convert.ToInt32(lastCode.WarehouseLocationCode.Substring(9, lastCode.WarehouseLocationCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var WarehouseLocation = new WarehouseLocation
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    WarehouseLocationId = vm.WarehouseLocationId,
                    WarehouseLocationCode = vm.WarehouseLocationCode,
                    WarehouseLocationName = vm.WarehouseLocationName,
                    WarehouseManagerId = vm.WarehouseManagerId,
                    Address = vm.Address
                };

                var checkDuplicate = _warehouseLocationRepository.GetAllWarehouseLocation().Where(c => c.WarehouseLocationName == vm.WarehouseLocationName).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _warehouseLocationRepository.GetAllWarehouseLocation().Where(c => c.WarehouseLocationName == vm.WarehouseLocationName).FirstOrDefault();
                    if (result == null)
                    {
                        _warehouseLocationRepository.Tambah(WarehouseLocation);
                        TempData["SuccessMessage"] = "Name " + vm.WarehouseLocationName + " Saved";
                        return RedirectToAction("Index", "WarehouseLocation");
                    }
                    else
                    {
                        ViewBag.WarehouseManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Name " + vm.WarehouseLocationName + " Already Exist !!!";
                        return View(vm);
                    }
                }
                else
                {
                    ViewBag.WarehouseManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Name " + vm.WarehouseLocationName + " Already Exist !!!";
                    return View(vm);
                }
            }
            else
            {
                ViewBag.WarehouseManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);                
                return View(vm);
            }
        }

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:MasterData/WarehouseLocation/DetailWarehouseLocation/{Id}";
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

        [HttpGet]
        public async Task<IActionResult> DetailWarehouseLocation(Guid Id)
        {
            ViewBag.Active = "MasterData";

            ViewBag.WarehouseManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);

            var WarehouseLocation = await _warehouseLocationRepository.GetWarehouseLocationById(Id);

            if (WarehouseLocation == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            WarehouseLocationViewModel viewModel = new WarehouseLocationViewModel
            {
                WarehouseLocationId = WarehouseLocation.WarehouseLocationId,
                WarehouseLocationCode = WarehouseLocation.WarehouseLocationCode,
                WarehouseLocationName = WarehouseLocation.WarehouseLocationName,
                WarehouseManagerId = WarehouseLocation.WarehouseManagerId,
                Address = WarehouseLocation.Address
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailWarehouseLocation(WarehouseLocationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var WarehouseLocation = await _warehouseLocationRepository.GetWarehouseLocationByIdNoTracking(viewModel.WarehouseLocationId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var checkDuplicate = _warehouseLocationRepository.GetAllWarehouseLocation().Where(d => d.WarehouseLocationName == viewModel.WarehouseLocationName).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var data = _warehouseLocationRepository.GetAllWarehouseLocation().Where(d => d.WarehouseLocationCode == viewModel.WarehouseLocationCode).FirstOrDefault();

                    if (data != null)
                    {
                        WarehouseLocation.UpdateDateTime = DateTime.Now;
                        WarehouseLocation.UpdateBy = new Guid(getUser.Id);
                        WarehouseLocation.WarehouseLocationCode = viewModel.WarehouseLocationCode;
                        WarehouseLocation.WarehouseLocationName = viewModel.WarehouseLocationName;
                        WarehouseLocation.WarehouseManagerId = viewModel.WarehouseManagerId;
                        WarehouseLocation.Address = viewModel.Address;

                        _warehouseLocationRepository.Update(WarehouseLocation);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Name " + viewModel.WarehouseLocationName + " Success Changes";
                        return RedirectToAction("Index", "WarehouseLocation");
                    }
                    else
                    {
                        ViewBag.WarehouseManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Name " + viewModel.WarehouseLocationName + " Already Exist !!!";
                        return View(viewModel);
                    }
                }
                else
                {
                    ViewBag.WarehouseManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Name " + viewModel.WarehouseLocationName + " Already Exist !!!";
                    return View(viewModel);
                }
            }
            else
            {
                ViewBag.WarehouseManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);                
                return View(viewModel);
            }
        }

        public IActionResult RedirectToDelete(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Delete:MasterData/WarehouseLocation/DeleteWarehouseLocation/{Id}";
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

        [HttpGet]
        public async Task<IActionResult> DeleteWarehouseLocation(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var WarehouseLocation = await _warehouseLocationRepository.GetWarehouseLocationById(Id);
            if (WarehouseLocation == null)
            {
                Response.StatusCode = 404;
                return View("WarehouseLocationNotFound", Id);
            }

            WarehouseLocationViewModel vm = new WarehouseLocationViewModel
            {
                WarehouseLocationId = WarehouseLocation.WarehouseLocationId,
                WarehouseLocationCode = WarehouseLocation.WarehouseLocationCode,
                WarehouseLocationName = WarehouseLocation.WarehouseLocationName,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWarehouseLocation(WarehouseLocationViewModel vm)
        {
            //Cek Relasi
            var produk = _productRepository.GetAllProduct().Where(p => p.WarehouseLocationId == vm.WarehouseLocationId).FirstOrDefault();
            if (produk == null)
            {
                //Hapus Data
                var WarehouseLocation = _applicationDbContext.WarehouseLocations.FirstOrDefault(x => x.WarehouseLocationId == vm.WarehouseLocationId);
                _applicationDbContext.Attach(WarehouseLocation);
                _applicationDbContext.Entry(WarehouseLocation).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                TempData["SuccessMessage"] = "Name " + vm.WarehouseLocationName + " Success Deleted";
                return RedirectToAction("Index", "WarehouseLocation");
            }
            else
            {
                TempData["WarningMessage"] = "Sorry, " + vm.WarehouseLocationName + " In used by the product !";
                return View(vm);
            }
        }
    }
}
