using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Security.Cryptography;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class DepartmentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IDepartmentRepository _departmentRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DepartmentController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IDepartmentRepository DepartmentRepository,
            IUserActiveRepository userActiveRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _departmentRepository = DepartmentRepository;
            _userActiveRepository = userActiveRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
            _hostingEnvironment = hostingEnvironment;
        }

        //[Authorize(Roles = "IndexDepartment")]       
        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:MasterData/Department/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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
        
        //[Authorize(Roles = "IndexDepartment")]
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

            var data = await _departmentRepository.GetAllDepartmentPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<Department>
            {
                Items = data.departments,
                TotalCount = data.totalCountDepartments,
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

        //[Authorize(Roles = "CreateDepartment")]
        public IActionResult RedirectToCreate()
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Create:MasterData/Department/CreateDepartment";
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
        
        //[Authorize(Roles = "CreateDepartment")]
        public async Task<ViewResult> CreateDepartment()
        {
            ViewBag.Active = "MasterData";
            var user = new DepartmentViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _departmentRepository.GetAllDepartment().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.DepartmentCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.DepartmentCode = "DPT" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.DepartmentCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.DepartmentCode = "DPT" + setDateNow + "0001";
                }
                else
                {
                    user.DepartmentCode = "DPT" + setDateNow + (Convert.ToInt32(lastCode.DepartmentCode.Substring(9, lastCode.DepartmentCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        //[Authorize(Roles = "CreateDepartment")]
        public async Task<IActionResult> CreateDepartment(DepartmentViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _departmentRepository.GetAllDepartment().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.DepartmentCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.DepartmentCode = "DPT" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.DepartmentCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.DepartmentCode = "DPT" + setDateNow + "0001";
                }
                else
                {
                    vm.DepartmentCode = "DPT" + setDateNow + (Convert.ToInt32(lastCode.DepartmentCode.Substring(9, lastCode.DepartmentCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var Department = new Department
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    DepartmentId = vm.DepartmentId,
                    DepartmentCode = vm.DepartmentCode,
                    DepartmentName = vm.DepartmentName,
                    Note = vm.Note
                };

                var checkDuplicate = _departmentRepository.GetAllDepartment().Where(c => c.DepartmentName == vm.DepartmentName).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _departmentRepository.GetAllDepartment().Where(c => c.DepartmentName == vm.DepartmentName).FirstOrDefault();
                    if (result == null)
                    {
                        _departmentRepository.Tambah(Department);
                        TempData["SuccessMessage"] = "Name " + vm.DepartmentName + " Saved";
                        return RedirectToAction("Index", "Department");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Name " + vm.DepartmentName + " Already Exist !!!";
                        return View(vm);
                    }
                }
                else 
                {
                    TempData["WarningMessage"] = "Name " + vm.DepartmentName + " There is duplicate data !!!";
                    return View(vm);
                }
            } 
            else
            {
                return View(vm);
            }
        }

        //[Authorize(Roles = "DetailDepartment")]
        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:MasterData/Department/DetailDepartment/{Id}";
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
        
        //[Authorize(Roles = "DetailDepartment")]
        public async Task<IActionResult> DetailDepartment(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var Department = await _departmentRepository.GetDepartmentById(Id);

            if (Department == null)
            {
                Response.StatusCode = 404;
                return View("DepartmentNotFound", Id);
            }

            DepartmentViewModel viewModel = new DepartmentViewModel
            {
                DepartmentId = Department.DepartmentId,
                DepartmentCode = Department.DepartmentCode,
                DepartmentName = Department.DepartmentName,
                Note = Department.Note
            };
            return View(viewModel);
        }

        [HttpPost]
        //[Authorize(Roles = "DetailDepartment")]
        public async Task<IActionResult> DetailDepartment(DepartmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Department = await _departmentRepository.GetDepartmentByIdNoTracking(viewModel.DepartmentId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var checkDuplicate = _departmentRepository.GetAllDepartment().Where(d => d.DepartmentName == viewModel.DepartmentName).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var data = _departmentRepository.GetAllDepartment().Where(d => d.DepartmentCode == viewModel.DepartmentCode).FirstOrDefault();

                    if (data != null)
                    {
                        Department.UpdateDateTime = DateTime.Now;
                        Department.UpdateBy = new Guid(getUser.Id);
                        Department.DepartmentCode = viewModel.DepartmentCode;
                        Department.DepartmentName = viewModel.DepartmentName;
                        Department.Note = viewModel.Note;

                        _departmentRepository.Update(Department);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Name " + viewModel.DepartmentName + " Success Changes";
                        return RedirectToAction("Index", "Department");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Name " + viewModel.DepartmentName + " Already Exist !!!";
                        return View(viewModel);
                    }
                }
                else 
                {
                    TempData["WarningMessage"] = "Name " + viewModel.DepartmentName + " There is duplicate data !!!";
                    return View(viewModel);
                }
            }
            else 
            {
                return View(viewModel);
            }            
        }

        //[Authorize(Roles = "DeleteDepartment")]
        public IActionResult RedirectToDelete(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Delete:MasterData/Department/DeleteDepartment/{Id}";
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
        
        //[Authorize(Roles = "DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartment(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var Department = await _departmentRepository.GetDepartmentById(Id);
            if (Department == null)
            {
                Response.StatusCode = 404;
                return View("DepartmentNotFound", Id);
            }

            DepartmentViewModel vm = new DepartmentViewModel
            {
                DepartmentId = Department.DepartmentId,
                DepartmentCode = Department.DepartmentCode,
                DepartmentName = Department.DepartmentName,
            };
            return View(vm);
        }

        [HttpPost]
        //[Authorize(Roles = "DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartment(DepartmentViewModel vm)
        {
            //Cek Relasi Principal dengan Produk
            var Department = _applicationDbContext.Departments.FirstOrDefault(x => x.DepartmentId == vm.DepartmentId);
            _applicationDbContext.Attach(Department);
            _applicationDbContext.Entry(Department).State = EntityState.Deleted;
            _applicationDbContext.SaveChanges();

            TempData["SuccessMessage"] = "Name " + vm.DepartmentName + " Success Deleted";
            return RedirectToAction("Index", "Department");
        }
    }
}
