using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using System.Data.SqlClient;
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

        private readonly IHostingEnvironment _hostingEnvironment;

        public PositionController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IPositionRepository PositionRepository,
            IUserActiveRepository userActiveRepository,
            IDepartmentRepository departmentRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _positionRepository = PositionRepository;
            _userActiveRepository = userActiveRepository;
            _departmentRepository = departmentRepository;

            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "MasterData";
            var data = _positionRepository.GetAllPosition();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime? tglAwalPencarian, DateTime? tglAkhirPencarian, string filterOptions)
        {
            ViewBag.Active = "MasterData";

            var data = _positionRepository.GetAllPosition();

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
