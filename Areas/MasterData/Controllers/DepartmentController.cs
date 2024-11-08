﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
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

        private readonly IHostingEnvironment _hostingEnvironment;

        public DepartmentController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IDepartmentRepository DepartmentRepository,
            IUserActiveRepository userActiveRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _departmentRepository = DepartmentRepository;
            _userActiveRepository = userActiveRepository;

            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "MasterData";
            var data = _departmentRepository.GetAllDepartment();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime? tglAwalPencarian, DateTime? tglAkhirPencarian, string filterOptions)
        {
            ViewBag.Active = "MasterData";

            var data = _departmentRepository.GetAllDepartment();

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

        [HttpGet]
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

        [HttpGet]
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
