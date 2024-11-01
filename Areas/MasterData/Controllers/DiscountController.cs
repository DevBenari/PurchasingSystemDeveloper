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
    public class DiscountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IProductRepository _productRepository;

        private readonly IHostingEnvironment _hostingEnvironment;

        public DiscountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IDiscountRepository DiscountRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _discountRepository = DiscountRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "MasterData";
            var data = _discountRepository.GetAllDiscount();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime? tglAwalPencarian, DateTime? tglAkhirPencarian, string filterOptions)
        {
            ViewBag.Active = "MasterData";

            var data = _discountRepository.GetAllDiscount();

            if (tglAwalPencarian.HasValue && tglAkhirPencarian.HasValue)
            {
                data = data.Where(u => u.CreateDateTime.Date >= tglAwalPencarian.Value.Date &&
                                       u.CreateDateTime.Date <= tglAkhirPencarian.Value.Date);
            }
            else if (!string.IsNullOrEmpty(filterOptions))
            {
                var today = DateTime.Today;
                switch (filterOptions)
                {
                    case "Today":
                        data = data.Where(u => u.CreateDateTime.Date == today);
                        break;
                    case "Last Day":
                        data = data.Where(x => x.CreateDateTime.Date == today.AddDays(-1));
                        break;

                    case "Last 7 Days":
                        var last7Days = today.AddDays(-7);
                        data = data.Where(x => x.CreateDateTime.Date >= last7Days && x.CreateDateTime.Date <= today);
                        break;

                    case "Last 30 Days":
                        var last30Days = today.AddDays(-30);
                        data = data.Where(x => x.CreateDateTime.Date >= last30Days && x.CreateDateTime.Date <= today);
                        break;

                    case "This Month":
                        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                        data = data.Where(x => x.CreateDateTime.Date >= firstDayOfMonth && x.CreateDateTime.Date <= today);
                        break;

                    case "Last Month":
                        var firstDayOfLastMonth = today.AddMonths(-1).Date.AddDays(-(today.Day - 1));
                        var lastDayOfLastMonth = today.Date.AddDays(-today.Day);
                        data = data.Where(x => x.CreateDateTime.Date >= firstDayOfLastMonth && x.CreateDateTime.Date <= lastDayOfLastMonth);
                        break;
                    default:
                        break;
                }
            }

            ViewBag.tglAwalPencarian = tglAwalPencarian?.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian?.ToString("dd MMMM yyyy");
            ViewBag.SelectedFilter = filterOptions;

            return View(data);
        }

        [HttpGet]
        public async Task<ViewResult> CreateDiscount()
        {
            ViewBag.Active = "MasterData";
            var user = new DiscountViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _discountRepository.GetAllDiscount().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.DiscountCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.DiscountCode = "DSC" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.DiscountCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.DiscountCode = "DSC" + setDateNow + "0001";
                }
                else
                {
                    user.DiscountCode = "DSC" + setDateNow + (Convert.ToInt32(lastCode.DiscountCode.Substring(9, lastCode.DiscountCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount(DiscountViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _discountRepository.GetAllDiscount().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.DiscountCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.DiscountCode = "DSC" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.DiscountCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.DiscountCode = "DSC" + setDateNow + "0001";
                }
                else
                {
                    vm.DiscountCode = "DSC" + setDateNow + (Convert.ToInt32(lastCode.DiscountCode.Substring(9, lastCode.DiscountCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var Discount = new Discount
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    DiscountId = vm.DiscountId,
                    DiscountCode = vm.DiscountCode,
                    DiscountValue = vm.DiscountValue,
                    Note = vm.Note
                };

                var checkDuplicate = _discountRepository.GetAllDiscount().Where(c => c.DiscountValue == vm.DiscountValue).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _discountRepository.GetAllDiscount().Where(c => c.DiscountValue == vm.DiscountValue).FirstOrDefault();
                    if (result == null)
                    {
                        _discountRepository.Tambah(Discount);
                        TempData["SuccessMessage"] = "Discount " + vm.DiscountValue + " % Saved";
                        return RedirectToAction("Index", "Discount");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Discount " + vm.DiscountValue + " % Already Exist !!!";
                        return View(vm);
                    }
                }
                else 
                {
                    TempData["WarningMessage"] = "Discount " + vm.DiscountValue + " % There is duplicate data !!!";
                    return View(vm);
                }
            }
            else
            {                
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailDiscount(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var Discount = await _discountRepository.GetDiscountById(Id);

            if (Discount == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            DiscountViewModel viewModel = new DiscountViewModel
            {
                DiscountId = Discount.DiscountId,
                DiscountCode = Discount.DiscountCode,
                DiscountValue = Discount.DiscountValue,
                Note = Discount.Note
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailDiscount(DiscountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Discount = await _discountRepository.GetDiscountByIdNoTracking(viewModel.DiscountId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();                
                var checkDuplicate = _discountRepository.GetAllDiscount().Where(d => d.DiscountValue == viewModel.DiscountValue).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var data = _discountRepository.GetAllDiscount().Where(d => d.DiscountCode == viewModel.DiscountCode).FirstOrDefault();

                    if (data != null)
                    {
                        Discount.UpdateDateTime = DateTime.Now;
                        Discount.UpdateBy = new Guid(getUser.Id);
                        Discount.DiscountCode = viewModel.DiscountCode;
                        Discount.DiscountValue = viewModel.DiscountValue;
                        Discount.Note = viewModel.Note;

                        _discountRepository.Update(Discount);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Discount " + viewModel.DiscountValue + " % Success Changes";
                        return RedirectToAction("Index", "Discount");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Discount " + viewModel.DiscountValue + " % Already Exist !!!";
                        return View(viewModel);
                    }
                }
                else
                {
                    TempData["WarningMessage"] = "Discount " + viewModel.DiscountValue + " % There is duplicate data !!!";
                    return View(viewModel);
                }
            } 
            else 
            {
                return View(viewModel);
            }            
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDiscount(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var Discount = await _discountRepository.GetDiscountById(Id);
            if (Discount == null)
            {
                Response.StatusCode = 404;
                return View("DiscountNotFound", Id);
            }

            DiscountViewModel vm = new DiscountViewModel
            {
                DiscountId = Discount.DiscountId,
                DiscountCode = Discount.DiscountCode,
                DiscountValue = Discount.DiscountValue,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDiscount(DiscountViewModel vm)
        {
            //Cek Relasi
            var produk = _productRepository.GetAllProduct().Where(p => p.DiscountId == vm.DiscountId).FirstOrDefault();
            if (produk == null)
            {
                //Hapus Data
                var Discount = _applicationDbContext.Discounts.FirstOrDefault(x => x.DiscountId == vm.DiscountId);
                _applicationDbContext.Attach(Discount);
                _applicationDbContext.Entry(Discount).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                TempData["SuccessMessage"] = "Discount " + vm.DiscountValue + " % Success Deleted";
                return RedirectToAction("Index", "Discount");
            }
            else {
                TempData["WarningMessage"] = "Sorry, " + vm.DiscountValue + " In used by the product !";
                return View(vm);
            }  
        }
    }
}
