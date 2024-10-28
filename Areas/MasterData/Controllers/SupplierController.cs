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
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class SupplierController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILeadTimeRepository _leadTimeRepository;

        private readonly IHostingEnvironment _hostingEnvironment;

        public SupplierController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            ISupplierRepository SupplierRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ILeadTimeRepository leadTimeRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _supplierRepository = SupplierRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _leadTimeRepository = leadTimeRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "MasterData";
            var data = _supplierRepository.GetAllSupplier();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "MasterData";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _supplierRepository.GetAllSupplier().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> NonActive()
        {
            ViewBag.Active = "MasterData";
            var data = _supplierRepository.GetAllSupplierNonActive();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> NonActive(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "MasterData";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _supplierRepository.GetAllSupplierNonActive().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> NonPks()
        {
            ViewBag.Active = "MasterData";
            var data = _supplierRepository.GetAllSupplierNonPks();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> NonPks(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "MasterData";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _supplierRepository.GetAllSupplierNonPks().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<ViewResult> CreateSupplier()
        {
            ViewBag.Active = "MasterData";

            ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);

            var user = new SupplierViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _supplierRepository.GetAllSupplier().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.SupplierCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.SupplierCode = "PCP" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.SupplierCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.SupplierCode = "PCP" + setDateNow + "0001";
                }
                else
                {
                    user.SupplierCode = "PCP" + setDateNow + (Convert.ToInt32(lastCode.SupplierCode.Substring(9, lastCode.SupplierCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier(SupplierViewModel vm)
        {
            ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _supplierRepository.GetAllSupplier().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.SupplierCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.SupplierCode = "PCP" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.SupplierCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.SupplierCode = "PCP" + setDateNow + "0001";
                }
                else
                {
                    vm.SupplierCode = "PCP" + setDateNow + (Convert.ToInt32(lastCode.SupplierCode.Substring(9, lastCode.SupplierCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var Supplier = new Supplier
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    SupplierId = vm.SupplierId,
                    SupplierCode = vm.SupplierCode,
                    SupplierName = vm.SupplierName,
                    LeadTimeId = vm.LeadTimeId,
                    Address = vm.Address,
                    Handphone = vm.Handphone,
                    Email = vm.Email,
                    Note = vm.Note,
                    IsPKS = vm.IsPKS,
                    IsActive = true,
                };
              
                var checkDuplicate = _supplierRepository.GetAllSupplier().Where(c => c.SupplierName == vm.SupplierName).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _supplierRepository.GetAllSupplier().Where(c => c.SupplierName == vm.SupplierName).FirstOrDefault();
                    if (result == null)
                    {
                        _supplierRepository.Tambah(Supplier);
                        TempData["SuccessMessage"] = "Name " + vm.SupplierName + " Saved";
                        return RedirectToAction("Index", "Supplier");
                    }
                    else
                    {
                        ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Name " + vm.SupplierName + " Already Exist !!!";
                        return View(vm);
                    }
                }
                else 
                {
                    ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Name " + vm.SupplierName + " There is duplicate data !!!";
                    return View(vm);
                }
            }
            else
            {
                ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);               
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailSupplier(Guid Id)
        {
            ViewBag.Active = "MasterData";
            
            ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);

            var Supplier = await _supplierRepository.GetSupplierById(Id);

            if (Supplier == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            SupplierViewModel viewModel = new SupplierViewModel
            {
                SupplierId = Supplier.SupplierId,
                SupplierCode = Supplier.SupplierCode,
                SupplierName = Supplier.SupplierName,
                LeadTimeId = Supplier.LeadTimeId,
                Address = Supplier.Address,
                Handphone = Supplier.Handphone,
                Email = Supplier.Email,
                Note = Supplier.Note,
                IsPKS = Supplier.IsPKS,
                IsActive = Supplier.IsActive,
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailSupplier(SupplierViewModel viewModel)
        {
            ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);

            if (ModelState.IsValid)
            {
                var Supplier = await _supplierRepository.GetSupplierByIdNoTracking(viewModel.SupplierId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                if (Supplier.IsActive == false)
                {
                    var checkDuplicate = _supplierRepository.GetAllSupplierNonActive().Where(d => d.SupplierName == viewModel.SupplierName).ToList();

                    if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                    {
                        var data = _supplierRepository.GetAllSupplierNonActive().Where(d => d.SupplierCode == viewModel.SupplierCode).FirstOrDefault();

                        if (data != null)
                        {
                            Supplier.UpdateDateTime = DateTime.Now;
                            Supplier.UpdateBy = new Guid(getUser.Id);
                            Supplier.SupplierCode = viewModel.SupplierCode;
                            Supplier.SupplierName = viewModel.SupplierName;
                            Supplier.LeadTimeId = viewModel.LeadTimeId;
                            Supplier.Address = viewModel.Address;
                            Supplier.Handphone = viewModel.Handphone;
                            Supplier.Email = viewModel.Email;
                            Supplier.Note = viewModel.Note;
                            Supplier.IsPKS = viewModel.IsPKS;
                            Supplier.IsActive = viewModel.IsActive;

                            _supplierRepository.Update(Supplier);
                            _applicationDbContext.SaveChanges();

                            TempData["SuccessMessage"] = "Name " + viewModel.SupplierName + " Success Changes";
                            return RedirectToAction("NonActive", "Supplier");
                        }
                        else
                        {
                            ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);
                            TempData["WarningMessage"] = "Name " + viewModel.SupplierName + " Already Exist !!!";
                            return View(viewModel);
                        }
                    }
                    else
                    {
                        ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Name " + viewModel.SupplierName + " There is duplicate data !!!";
                        return View(viewModel);
                    }
                }
                else if (Supplier.IsPKS == false && Supplier.IsActive == true)
                {
                    var checkDuplicate = _supplierRepository.GetAllSupplierNonPks().Where(d => d.SupplierName == viewModel.SupplierName).ToList();

                    if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                    {
                        var data = _supplierRepository.GetAllSupplierNonPks().Where(d => d.SupplierCode == viewModel.SupplierCode).FirstOrDefault();

                        if (data != null)
                        {
                            Supplier.UpdateDateTime = DateTime.Now;
                            Supplier.UpdateBy = new Guid(getUser.Id);
                            Supplier.SupplierCode = viewModel.SupplierCode;
                            Supplier.SupplierName = viewModel.SupplierName;
                            Supplier.LeadTimeId = viewModel.LeadTimeId;
                            Supplier.Address = viewModel.Address;
                            Supplier.Handphone = viewModel.Handphone;
                            Supplier.Email = viewModel.Email;
                            Supplier.Note = viewModel.Note;
                            Supplier.IsPKS = viewModel.IsPKS;
                            Supplier.IsActive = viewModel.IsActive;

                            _supplierRepository.Update(Supplier);
                            _applicationDbContext.SaveChanges();

                            TempData["SuccessMessage"] = "Name " + viewModel.SupplierName + " Success Changes";
                            return RedirectToAction("NonPks", "Supplier");
                        }
                        else
                        {
                            ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);
                            TempData["WarningMessage"] = "Name " + viewModel.SupplierName + " Already Exist !!!";
                            return View(viewModel);
                        }
                    }
                    else
                    {
                        ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Name " + viewModel.SupplierName + " There is duplicate data !!!";
                        return View(viewModel);
                    }
                }                
                else if (Supplier.IsActive == true && Supplier.IsPKS == true)
                {
                    var checkDuplicate = _supplierRepository.GetAllSupplier().Where(d => d.SupplierName == viewModel.SupplierName).ToList();

                    if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                    {
                        var data = _supplierRepository.GetAllSupplier().Where(d => d.SupplierCode == viewModel.SupplierCode).FirstOrDefault();

                        if (data != null)
                        {
                            Supplier.UpdateDateTime = DateTime.Now;
                            Supplier.UpdateBy = new Guid(getUser.Id);
                            Supplier.SupplierCode = viewModel.SupplierCode;
                            Supplier.SupplierName = viewModel.SupplierName;
                            Supplier.LeadTimeId = viewModel.LeadTimeId;
                            Supplier.Address = viewModel.Address;
                            Supplier.Handphone = viewModel.Handphone;
                            Supplier.Email = viewModel.Email;
                            Supplier.Note = viewModel.Note;
                            Supplier.IsPKS = viewModel.IsPKS;
                            Supplier.IsActive = viewModel.IsActive;

                            _supplierRepository.Update(Supplier);
                            _applicationDbContext.SaveChanges();

                            TempData["SuccessMessage"] = "Name " + viewModel.SupplierName + " Success Changes";
                            return RedirectToAction("Index", "Supplier");
                        }
                        else
                        {
                            ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);
                            TempData["WarningMessage"] = "Name " + viewModel.SupplierName + " Already Exist !!!";
                            return View(viewModel);
                        }
                    }
                    else 
                    {
                        ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Name " + viewModel.SupplierName + " There is duplicate data !!!";
                        return View(viewModel);
                    }
                }
            }
            else
            {
                ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);                
                return View(viewModel);
            }

            ViewBag.LeadTime = new SelectList(await _leadTimeRepository.GetLeadTimes(), "LeadTimeId", "LeadTimeValue", SortOrder.Ascending);                
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSupplier(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var Supplier = await _supplierRepository.GetSupplierById(Id);
            if (Supplier == null)
            {
                Response.StatusCode = 404;
                return View("SupplierNotFound", Id);
            }

            SupplierViewModel vm = new SupplierViewModel
            {
                SupplierId = Supplier.SupplierId,
                SupplierCode = Supplier.SupplierCode,
                SupplierName = Supplier.SupplierName,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSupplier(SupplierViewModel vm)
        {
            //Cek Relasi
            var produk = _productRepository.GetAllProduct().Where(p => p.SupplierId == vm.SupplierId).FirstOrDefault();
            if (produk == null)
            {
                //Hapus Data
                var Supplier = _applicationDbContext.Suppliers.FirstOrDefault(x => x.SupplierId == vm.SupplierId);
                _applicationDbContext.Attach(Supplier);
                _applicationDbContext.Entry(Supplier).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                TempData["SuccessMessage"] = "Name " + vm.SupplierName + " Success Deleted";
                return RedirectToAction("Index", "Supplier");
            }
            else {
                TempData["WarningMessage"] = "Sorry, " + vm.SupplierName + " In used by the product !";
                return View(vm);
            }
        }
    }
}
