using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class PositionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IDepartmentRepository _departmentRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PositionController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IPositionRepository PositionRepository,
            IUserActiveRepository userActiveRepository,
            IDepartmentRepository departmentRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _positionRepository = PositionRepository;
            _userActiveRepository = userActiveRepository;
            _departmentRepository = departmentRepository;

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
                string originalPath = $"Page:MasterData/Position/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

            var data = await _positionRepository.GetAllPositionPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<Position>
            {
                Items = data.positions,
                TotalCount = data.totalCountPositions,
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
                string originalPath = $"Create:MasterData/Position/CreatePosition";
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
        public async Task<ViewResult> CreatePosition()
        {
            ViewBag.Active = "MasterData";
            
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);

            var user = new PositionViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _positionRepository.GetAllPosition().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.PositionCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.PositionCode = "PST" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.PositionCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.PositionCode = "PST" + setDateNow + "0001";
                }
                else
                {
                    user.PositionCode = "PST" + setDateNow + (Convert.ToInt32(lastCode.PositionCode.Substring(9, lastCode.PositionCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePosition(PositionViewModel vm)
        {
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _positionRepository.GetAllPosition().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.PositionCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.PositionCode = "PST" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.PositionCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.PositionCode = "PST" + setDateNow + "0001";
                }
                else
                {
                    vm.PositionCode = "PST" + setDateNow + (Convert.ToInt32(lastCode.PositionCode.Substring(9, lastCode.PositionCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var Position = new Position
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    PositionId = vm.PositionId,
                    PositionCode = vm.PositionCode,
                    PositionName = vm.PositionName,
                    DepartmentId = vm.DepartmentId,
                };

                var checkDuplicate = _positionRepository.GetAllPosition().Where(c => c.PositionName == vm.PositionName && c.DepartmentId == vm.DepartmentId).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _positionRepository.GetAllPosition().Where(c => c.PositionName == vm.PositionName && c.DepartmentId == vm.DepartmentId).FirstOrDefault();
                    if (result == null)
                    {
                        _positionRepository.Tambah(Position);
                        TempData["SuccessMessage"] = "Name " + vm.PositionName + " Saved";
                        return RedirectToAction("Index", "Position");
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Name " + vm.PositionName + " Already Exist !!!";
                        return View(vm);
                    }
                }
                else
                {
                    ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Name " + vm.PositionName + " There is duplicate data !!!";
                    return View(vm);
                }
            }
            else 
            {
                ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                return View(vm);
            }
        }

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:MasterData/Position/DetailPosition/{Id}";
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
        public async Task<IActionResult> DetailPosition(Guid Id)
        {
            ViewBag.Active = "MasterData";

            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);

            var Position = await _positionRepository.GetPositionById(Id);

            if (Position == null)
            {
                Response.StatusCode = 404;
                return View("PositionNotFound", Id);
            }

            PositionViewModel viewModel = new PositionViewModel
            {
                PositionId = Position.PositionId,
                PositionCode = Position.PositionCode,
                PositionName = Position.PositionName,
                DepartmentId = Position.DepartmentId,
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailPosition(PositionViewModel viewModel)
        {
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);

            if (ModelState.IsValid)
            {
                var Position = await _positionRepository.GetPositionByIdNoTracking(viewModel.PositionId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var checkDuplicate = _positionRepository.GetAllPosition().Where(d => d.PositionName == viewModel.PositionName && d.DepartmentId == viewModel.DepartmentId).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var check = _positionRepository.GetAllPosition().Where(d => d.PositionCode == viewModel.PositionCode).FirstOrDefault();

                    if (check != null)
                    {
                        Position.UpdateDateTime = DateTime.Now;
                        Position.UpdateBy = new Guid(getUser.Id);
                        Position.PositionCode = viewModel.PositionCode;
                        Position.PositionName = viewModel.PositionName;
                        Position.DepartmentId = viewModel.DepartmentId;

                        _positionRepository.Update(Position);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Name " + viewModel.PositionName + " Success Changes";
                        return RedirectToAction("Index", "Position");
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Name " + viewModel.PositionName + " Already Exist !!!";
                        return View(viewModel);
                    }
                }
                else
                {
                    ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Name " + viewModel.PositionName + " There is duplicate data !!!";
                    return View(viewModel);
                }
            }
            else
            {
                ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);               
                return View(viewModel);
            }
        }

        public IActionResult RedirectToDelete(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Delete:MasterData/Position/DeletePosition/{Id}";
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
        public async Task<IActionResult> DeletePosition(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var Position = await _positionRepository.GetPositionById(Id);
            if (Position == null)
            {
                Response.StatusCode = 404;
                return View("PositionNotFound", Id);
            }

            PositionViewModel vm = new PositionViewModel
            {
                PositionId = Position.PositionId,
                PositionCode = Position.PositionCode,
                PositionName = Position.PositionName,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePosition(PositionViewModel vm)
        {
            //Cek Relasi Principal dengan Produk
            var Position = _applicationDbContext.Positions.FirstOrDefault(x => x.PositionId == vm.PositionId);
            _applicationDbContext.Attach(Position);
            _applicationDbContext.Entry(Position).State = EntityState.Deleted;
            _applicationDbContext.SaveChanges();

            TempData["SuccessMessage"] = "Name " + vm.PositionName + " Success Deleted";
            return RedirectToAction("Index", "Position");
        }
    }
}
