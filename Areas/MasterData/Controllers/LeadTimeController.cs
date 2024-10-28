using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class LeadTimeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly ILeadTimeRepository _leadTimeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInitialStockRepository _initialStockRepository;

        private readonly IHostingEnvironment _hostingEnvironment;

        public LeadTimeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            ILeadTimeRepository LeadTimeRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            IInitialStockRepository initialStockRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _leadTimeRepository = LeadTimeRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _initialStockRepository = initialStockRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "MasterData";
            var data = _leadTimeRepository.GetAllLeadTime();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "MasterData";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _leadTimeRepository.GetAllLeadTime().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<ViewResult> CreateLeadTime()
        {
            ViewBag.Active = "MasterData";
            var user = new LeadTimeViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _leadTimeRepository.GetAllLeadTime().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.LeadTimeCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.LeadTimeCode = "LDT" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.LeadTimeCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.LeadTimeCode = "LDT" + setDateNow + "0001";
                }
                else
                {
                    user.LeadTimeCode = "LDT" + setDateNow + (Convert.ToInt32(lastCode.LeadTimeCode.Substring(9, lastCode.LeadTimeCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeadTime(LeadTimeViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _leadTimeRepository.GetAllLeadTime().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.LeadTimeCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.LeadTimeCode = "LDT" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.LeadTimeCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.LeadTimeCode = "LDT" + setDateNow + "0001";
                }
                else
                {
                    vm.LeadTimeCode = "LDT" + setDateNow + (Convert.ToInt32(lastCode.LeadTimeCode.Substring(9, lastCode.LeadTimeCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var LeadTime = new LeadTime
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    LeadTimeId = vm.LeadTimeId,
                    LeadTimeCode = vm.LeadTimeCode,
                    LeadTimeValue = vm.LeadTimeValue
                };

                var checkDuplicate = _leadTimeRepository.GetAllLeadTime().Where(c => c.LeadTimeValue == vm.LeadTimeValue).ToList();
                
                if (checkDuplicate.Count == 0)
                {
                    var result = _leadTimeRepository.GetAllLeadTime().Where(c => c.LeadTimeValue == vm.LeadTimeValue).FirstOrDefault();
                    if (result == null)
                    {
                        _leadTimeRepository.Tambah(LeadTime);
                        TempData["SuccessMessage"] = "Value " + vm.LeadTimeValue + " Saved";
                        return RedirectToAction("Index", "LeadTime");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Value " + vm.LeadTimeValue + " Already Exist !!!";
                        return View(vm);
                    }
                }
                else
                {
                    TempData["WarningMessage"] = "Value " + vm.LeadTimeValue + " There is duplicate data !!!";
                    return View(vm);
                }
            }
            else
            {               
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailLeadTime(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var LeadTime = await _leadTimeRepository.GetLeadTimeById(Id);

            if (LeadTime == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            LeadTimeViewModel viewModel = new LeadTimeViewModel
            {
                LeadTimeId = LeadTime.LeadTimeId,
                LeadTimeCode = LeadTime.LeadTimeCode,
                LeadTimeValue = LeadTime.LeadTimeValue
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailLeadTime(LeadTimeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var LeadTime = await _leadTimeRepository.GetLeadTimeByIdNoTracking(viewModel.LeadTimeId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var checkDuplicate = _leadTimeRepository.GetAllLeadTime().Where(d => d.LeadTimeValue == viewModel.LeadTimeValue).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var data = _leadTimeRepository.GetAllLeadTime().Where(d => d.LeadTimeCode == viewModel.LeadTimeCode).FirstOrDefault();

                    if (data != null)
                    {
                        LeadTime.UpdateDateTime = DateTime.Now;
                        LeadTime.UpdateBy = new Guid(getUser.Id);
                        LeadTime.LeadTimeCode = viewModel.LeadTimeCode;
                        LeadTime.LeadTimeValue = viewModel.LeadTimeValue;

                        _leadTimeRepository.Update(LeadTime);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Value " + viewModel.LeadTimeValue + " Success Changes";
                        return RedirectToAction("Index", "LeadTime");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Value " + viewModel.LeadTimeValue + " Already Exist !!!";
                        return View(viewModel);
                    }
                }
                else
                {
                    TempData["WarningMessage"] = "Value " + viewModel.LeadTimeValue + " There is duplicate data !!!";
                    return View(viewModel);
                }
            }
            else
            {
                return View(viewModel);
            }            
        }

        [HttpGet]
        public async Task<IActionResult> DeleteLeadTime(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var LeadTime = await _leadTimeRepository.GetLeadTimeById(Id);
            if (LeadTime == null)
            {
                Response.StatusCode = 404;
                return View("LeadTimeNotFound", Id);
            }

            LeadTimeViewModel vm = new LeadTimeViewModel
            {
                LeadTimeId = LeadTime.LeadTimeId,
                LeadTimeCode = LeadTime.LeadTimeCode,
                LeadTimeValue = LeadTime.LeadTimeValue,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLeadTime(LeadTimeViewModel vm)
        {
            //Cek Relasi
            var initialstock = _initialStockRepository.GetAllInitialStock().Where(p => p.LeadTimeId == vm.LeadTimeId).FirstOrDefault();
            if (initialstock == null)
            {
                //Hapus Data
                var LeadTime = _applicationDbContext.LeadTimes.FirstOrDefault(x => x.LeadTimeId == vm.LeadTimeId);
                _applicationDbContext.Attach(LeadTime);
                _applicationDbContext.Entry(LeadTime).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                TempData["SuccessMessage"] = "Name " + vm.LeadTimeValue + " Success Deleted";
                return RedirectToAction("Index", "LeadTime");
            }
            else
            {
                TempData["WarningMessage"] = "Sorry, " + vm.LeadTimeValue + " In used by the initial stock !";
                return View(vm);
            }
        }
    }
}
