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
    public class TermOfPaymentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public TermOfPaymentController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            ITermOfPaymentRepository TermOfPaymentRepository,
            IUserActiveRepository userActiveRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _termOfPaymentRepository = TermOfPaymentRepository;
            _userActiveRepository = userActiveRepository;

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
                string originalPath = $"Page:MasterData/TermOfPayment/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

            var data = await _termOfPaymentRepository.GetAllTermOfPaymentPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<TermOfPayment>
            {
                Items = data.termOfPayments,
                TotalCount = data.totalCountTermOfPayments,
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
                string originalPath = $"Create:MasterData/TermOfPayment/CreateTermOfPayment";
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
        public async Task<ViewResult> CreateTermOfPayment()
        {
            ViewBag.Active = "MasterData";
            var user = new TermOfPaymentViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _termOfPaymentRepository.GetAllTermOfPayment().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.TermOfPaymentCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.TermOfPaymentCode = "TOP" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.TermOfPaymentCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.TermOfPaymentCode = "TOP" + setDateNow + "0001";
                }
                else
                {
                    user.TermOfPaymentCode = "TOP" + setDateNow + (Convert.ToInt32(lastCode.TermOfPaymentCode.Substring(9, lastCode.TermOfPaymentCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTermOfPayment(TermOfPaymentViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _termOfPaymentRepository.GetAllTermOfPayment().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.TermOfPaymentCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.TermOfPaymentCode = "TOP" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.TermOfPaymentCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.TermOfPaymentCode = "TOP" + setDateNow + "0001";
                }
                else
                {
                    vm.TermOfPaymentCode = "TOP" + setDateNow + (Convert.ToInt32(lastCode.TermOfPaymentCode.Substring(9, lastCode.TermOfPaymentCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var TermOfPayment = new TermOfPayment
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    TermOfPaymentId = vm.TermOfPaymentId,
                    TermOfPaymentCode = vm.TermOfPaymentCode,
                    TermOfPaymentName = vm.TermOfPaymentName,
                    Note = vm.Note
                };

                var checkDuplicate = _termOfPaymentRepository.GetAllTermOfPayment().Where(c => c.TermOfPaymentName == vm.TermOfPaymentName).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _termOfPaymentRepository.GetAllTermOfPayment().Where(c => c.TermOfPaymentName == vm.TermOfPaymentName).FirstOrDefault();
                    if (result == null)
                    {
                        _termOfPaymentRepository.Tambah(TermOfPayment);
                        TempData["SuccessMessage"] = "Name " + vm.TermOfPaymentName + " Saved";
                        return RedirectToAction("Index", "TermOfPayment");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Name " + vm.TermOfPaymentName + " Already Exist !!!";
                        return View(vm);
                    }
                } 
                else 
                {
                    TempData["WarningMessage"] = "Name " + vm.TermOfPaymentName + " There is duplicate data !!!";
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:MasterData/TermOfPayment/DetailTermOfPayment/{Id}";
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
        public async Task<IActionResult> DetailTermOfPayment(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var TermOfPayment = await _termOfPaymentRepository.GetTermOfPaymentById(Id);

            if (TermOfPayment == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            TermOfPaymentViewModel viewModel = new TermOfPaymentViewModel
            {
                TermOfPaymentId = TermOfPayment.TermOfPaymentId,
                TermOfPaymentCode = TermOfPayment.TermOfPaymentCode,
                TermOfPaymentName = TermOfPayment.TermOfPaymentName,
                Note = TermOfPayment.Note
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailTermOfPayment(TermOfPaymentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var TermOfPayment = await _termOfPaymentRepository.GetTermOfPaymentByIdNoTracking(viewModel.TermOfPaymentId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var checkDuplicate = _termOfPaymentRepository.GetAllTermOfPayment().Where(d => d.TermOfPaymentName == viewModel.TermOfPaymentName).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var data = _termOfPaymentRepository.GetAllTermOfPayment().Where(d => d.TermOfPaymentCode == viewModel.TermOfPaymentCode).FirstOrDefault();

                    if (data != null)
                    {
                        TermOfPayment.UpdateDateTime = DateTime.Now;
                        TermOfPayment.UpdateBy = new Guid(getUser.Id);
                        TermOfPayment.TermOfPaymentCode = viewModel.TermOfPaymentCode;
                        TermOfPayment.TermOfPaymentName = viewModel.TermOfPaymentName;
                        TermOfPayment.Note = viewModel.Note;

                        _termOfPaymentRepository.Update(TermOfPayment);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Name " + viewModel.TermOfPaymentName + " Success Changes";
                        return RedirectToAction("Index", "TermOfPayment");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Name " + viewModel.TermOfPaymentName + " Already Exist !!!";
                        return View(viewModel);
                    }
                }
                else
                {
                    TempData["WarningMessage"] = "Name " + viewModel.TermOfPaymentName + " There is duplicate data !!!";
                    return View(viewModel);
                }
            }
            else
            {                
                return View(viewModel);
            }
        }

        public IActionResult RedirectToDelete(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Delete:MasterData/TermOfPayment/DeleteTermOfPayment/{Id}";
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
        public async Task<IActionResult> DeleteTermOfPayment(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var TermOfPayment = await _termOfPaymentRepository.GetTermOfPaymentById(Id);
            if (TermOfPayment == null)
            {
                Response.StatusCode = 404;
                return View("TermOfPaymentNotFound", Id);
            }

            TermOfPaymentViewModel vm = new TermOfPaymentViewModel
            {
                TermOfPaymentId = TermOfPayment.TermOfPaymentId,
                TermOfPaymentCode = TermOfPayment.TermOfPaymentCode,
                TermOfPaymentName = TermOfPayment.TermOfPaymentName,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTermOfPayment(TermOfPaymentViewModel vm)
        {
            //Hapus Data Profil
            var TermOfPayment = _applicationDbContext.TermOfPayments.FirstOrDefault(x => x.TermOfPaymentId == vm.TermOfPaymentId);
            _applicationDbContext.Attach(TermOfPayment);
            _applicationDbContext.Entry(TermOfPayment).State = EntityState.Deleted;
            _applicationDbContext.SaveChanges();

            TempData["SuccessMessage"] = "Name " + vm.TermOfPaymentName + " Success Deleted";
            return RedirectToAction("Index", "TermOfPayment");
        }
    }
}
