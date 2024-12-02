using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Order.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using System.Net.Mail;
using System.Net;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.DataProtection;
using PurchasingSystemDeveloper.Repositories;
using System.Security.Cryptography;
using System.Text;
using PurchasingSystemDeveloper.Areas.MasterData.Models;

namespace PurchasingSystemDeveloper.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("Order/[Controller]/[Action]")]
    public class EmailController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IEmailRepository _emailRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmailController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IEmailRepository emailRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;
            _emailRepository = emailRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                ViewBag.Active = "PurchaseOrder";
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:Order/Email/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

        public async Task<IActionResult> Index(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            ViewBag.Active = "PurchaseOrder";
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

            var data = await _emailRepository.GetAllEmailPageSize(searchTerm, page, pageSize, startDate, endDate); // Ambil semua email dari repository

            var model = new Pagination<Email>
            {
                Items = data.emails,
                TotalCount = data.totalCountEmails,
                PageSize = pageSize,
                CurrentPage = page,
            };

            return View(model);
        }

        public IActionResult RedirectToCreate()
        {
            try
            {
                ViewBag.Active = "PurchaseOrder";
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Create:Order/Email/CreateEmail";
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
        public async Task<ViewResult> CreateEmail()
        {
            ViewBag.Active = "PurchaseOrder";
            var email = new EmailViewModel(); // Gunakan EmailViewModel
            return View(email); // Mengirim model EmailViewModel ke tampilan
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmail(EmailViewModel vm)
        {
            ViewBag.Active = "PurchaseOrder";
            var getUser = _userActiveRepository.GetAllUserLogin().FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (ModelState.IsValid)
            {
                // Ambil informasi file dari input
                string uniqueFileName = ProcessUploadFile(vm);

                // Simpan informasi file ke database
                var email = new Email
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    EmailId = Guid.NewGuid(),
                    To = vm.To,
                    Subject = vm.Subject,
                    Message = vm.Message,
                    Document = uniqueFileName,
                    Status = "Terkirim",

                    /*AttachmentFileName = uniqueFileName*/ // Simpan nama file unik ke database
                };

                _emailRepository.Tambah(email);
                TempData["SuccessMessage"] = "Email to " + vm.To + " Saved";
                return RedirectToAction("RedirectToIndex", "Email");
            }

            return View(vm);
        }

        private string ProcessUploadFile(EmailViewModel model)
        {
            string uniqueFileName = null;
            if (model.Document != null)
            {
                string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "EmailDocument");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Document.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                model.Document.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            return uniqueFileName;
        }


    }
}
