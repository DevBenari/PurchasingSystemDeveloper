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

        private readonly IHostingEnvironment _hostingEnvironment;

        public EmailController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IEmailRepository emailRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;
            _emailRepository = emailRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var emails = _emailRepository.GetAllEmails(); // Ambil semua email dari repository
            return View(emails);
        }

        [HttpGet]
        public async Task<ViewResult> CreateEmail()
        {
            var model = new EmailViewModel(); // Gunakan EmailViewModel
            return View(model); // Mengirim model EmailViewModel ke tampilan
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmail(EmailViewModel model)
        {
            var getUser = _userActiveRepository.GetAllUserLogin().FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (ModelState.IsValid)
            {
                // Ambil informasi file dari input
                string uniqueFileName = ProcessUploadFile(model);

                // Simpan informasi file ke database
                var email = new Email
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    EmailId = Guid.NewGuid(),
                    To = model.To,
                    Subject = model.Subject,
                    Message = model.Message,
                    Document = uniqueFileName,
                    Status = "Terkirim",

                    /*AttachmentFileName = uniqueFileName*/ // Simpan nama file unik ke database
                };

                _emailRepository.Tambah(email);
                return RedirectToAction("Index");
                //var file = Request.Form.Files.FirstOrDefault();

                //if (file != null && file.Length > 0)
                //{
                // Buat nama file unik
                //var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/email", uniqueFileName);

                // Simpan file ke direktori
                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    file.CopyTo(stream);
                //}                    
                //}
            }

            return View(model);
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
