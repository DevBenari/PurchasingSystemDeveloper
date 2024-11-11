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
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class UnitLocationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IUnitLocationRepository _UnitLocationRepository;
        private readonly IProductRepository _productRepository;

        private readonly IHostingEnvironment _hostingEnvironment;

        public UnitLocationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUnitLocationRepository UnitLocationRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _UnitLocationRepository = UnitLocationRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "MasterData";
            var data = _UnitLocationRepository.GetAllUnitLocation();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime? tglAwalPencarian, DateTime? tglAkhirPencarian, string filterOptions)
        {
            ViewBag.Active = "MasterData";

            var data = _UnitLocationRepository.GetAllUnitLocation();            

            if (tglAwalPencarian.HasValue && tglAkhirPencarian.HasValue)
            {
                data = data.Where(u => u.CreateDateTime.Date >= tglAwalPencarian.Value.Date
                                    && u.CreateDateTime.Date <= tglAkhirPencarian.Value.Date);
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
        public async Task<ViewResult> CreateUnitLocation()
        {
            ViewBag.Active = "MasterData";

            ViewBag.UnitManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);

            var user = new UnitLocationViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _UnitLocationRepository.GetAllUnitLocation().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.UnitLocationCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.UnitLocationCode = "UNT" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.UnitLocationCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.UnitLocationCode = "UNT" + setDateNow + "0001";
                }
                else
                {
                    user.UnitLocationCode = "UNT" + setDateNow + (Convert.ToInt32(lastCode.UnitLocationCode.Substring(9, lastCode.UnitLocationCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUnitLocation(UnitLocationViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _UnitLocationRepository.GetAllUnitLocation().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.UnitLocationCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.UnitLocationCode = "UNT" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.UnitLocationCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.UnitLocationCode = "UNT" + setDateNow + "0001";
                }
                else
                {
                    vm.UnitLocationCode = "UNT" + setDateNow + (Convert.ToInt32(lastCode.UnitLocationCode.Substring(9, lastCode.UnitLocationCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var UnitLocation = new UnitLocation
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    UnitLocationId = vm.UnitLocationId,
                    UnitLocationCode = vm.UnitLocationCode,
                    UnitLocationName = vm.UnitLocationName,
                    UnitManagerId = vm.UnitManagerId,
                    Address = vm.Address
                };

                var checkDuplicate = _UnitLocationRepository.GetAllUnitLocation().Where(c => c.UnitLocationName == vm.UnitLocationName).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _UnitLocationRepository.GetAllUnitLocation().Where(c => c.UnitLocationName == vm.UnitLocationName).FirstOrDefault();
                    if (result == null)
                    {
                        _UnitLocationRepository.Tambah(UnitLocation);
                        TempData["SuccessMessage"] = "Name " + vm.UnitLocationName + " Saved";
                        return RedirectToAction("Index", "UnitLocation");
                    }
                    else
                    {
                        ViewBag.UnitManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Name " + vm.UnitLocationName + " Already Exist !!!";
                        return View(vm);
                    }
                }
                else
                {
                    ViewBag.UnitManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Name " + vm.UnitLocationName + " There is duplicate data !!!";
                    return View(vm);
                }
            }
            else
            {
                ViewBag.UnitManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);                
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailUnitLocation(Guid Id)
        {
            ViewBag.Active = "MasterData";

            ViewBag.UnitManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);

            var UnitLocation = await _UnitLocationRepository.GetUnitLocationById(Id);

            if (UnitLocation == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            UnitLocationViewModel viewModel = new UnitLocationViewModel
            {
                UnitLocationId = UnitLocation.UnitLocationId,
                UnitLocationCode = UnitLocation.UnitLocationCode,
                UnitLocationName = UnitLocation.UnitLocationName,
                UnitManagerId = UnitLocation.UnitManagerId,
                Address = UnitLocation.Address
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailUnitLocation(UnitLocationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var UnitLocation = await _UnitLocationRepository.GetUnitLocationByIdNoTracking(viewModel.UnitLocationId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var checkDuplicate = _UnitLocationRepository.GetAllUnitLocation().Where(d => d.UnitLocationCode == viewModel.UnitLocationCode).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var check = _UnitLocationRepository.GetAllUnitLocation().Where(d => d.UnitLocationCode == viewModel.UnitLocationCode).FirstOrDefault();

                    if (check != null)
                    {
                        UnitLocation.UpdateDateTime = DateTime.Now;
                        UnitLocation.UpdateBy = new Guid(getUser.Id);
                        UnitLocation.UnitLocationCode = viewModel.UnitLocationCode;
                        UnitLocation.UnitLocationName = viewModel.UnitLocationName;
                        UnitLocation.UnitManagerId = viewModel.UnitManagerId;
                        UnitLocation.Address = viewModel.Address;

                        _UnitLocationRepository.Update(UnitLocation);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Name " + viewModel.UnitLocationName + " Success Changes";
                        return RedirectToAction("Index", "UnitLocation");
                    }
                    else
                    {
                        ViewBag.UnitManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);

                        TempData["WarningMessage"] = "Name " + viewModel.UnitLocationName + " Already Exist !!!";
                        return View(viewModel);
                    }
                }
                else
                {
                    ViewBag.UnitManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);

                    TempData["WarningMessage"] = "Name " + viewModel.UnitLocationName + " There is duplicate data !!!";
                    return View(viewModel);
                }
            }
            else
            {
                ViewBag.UnitManager = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);               
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUnitLocation(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var UnitLocation = await _UnitLocationRepository.GetUnitLocationById(Id);
            if (UnitLocation == null)
            {
                Response.StatusCode = 404;
                return View("UnitLocationNotFound", Id);
            }

            UnitLocationViewModel vm = new UnitLocationViewModel
            {
                UnitLocationId = UnitLocation.UnitLocationId,
                UnitLocationCode = UnitLocation.UnitLocationCode,
                UnitLocationName = UnitLocation.UnitLocationName,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUnitLocation(UnitLocationViewModel vm)
        {
            //Hapus Data
            var UnitLocation = _applicationDbContext.UnitLocations.FirstOrDefault(x => x.UnitLocationId == vm.UnitLocationId);
            _applicationDbContext.Attach(UnitLocation);
            _applicationDbContext.Entry(UnitLocation).State = EntityState.Deleted;
            _applicationDbContext.SaveChanges();

            TempData["SuccessMessage"] = "Name " + vm.UnitLocationName + " Success Deleted";
            return RedirectToAction("Index", "UnitLocation");
        }
    }
}
