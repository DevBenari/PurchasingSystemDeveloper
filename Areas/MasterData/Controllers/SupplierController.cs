using Microsoft.AspNetCore.DataProtection;
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
using System.IO;
using System.Security.Cryptography;
using System.Text;
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

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SupplierController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            ISupplierRepository SupplierRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ILeadTimeRepository leadTimeRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
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
                string originalPath = $"Page:MasterData/Supplier/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

            var data = await _supplierRepository.GetAllSupplierPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<Supplier>
            {
                Items = data.suppliers,
                TotalCount = data.totalCountSuppliers,
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

        public IActionResult RedirectToNonActive(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:MasterData/Supplier/NonActive?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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
        public async Task<IActionResult> NonActive(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
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

            var data = await _supplierRepository.GetAllSupplierNonActivePageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<Supplier>
            {
                Items = data.suppliers,
                TotalCount = data.totalCountSuppliers,
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

        public IActionResult RedirectToNonPks(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:MasterData/Supplier/NonPks?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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
        public async Task<IActionResult> NonPks(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
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

            var data = await _supplierRepository.GetAllSupplierNonPksPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<Supplier>
            {
                Items = data.suppliers,
                TotalCount = data.totalCountSuppliers,
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
                string originalPath = $"Create:MasterData/Supplier/CreateSupplier";
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

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:MasterData/Supplier/DetailSupplier/{Id}";
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

        public IActionResult RedirectToDelete(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Delete:MasterData/Supplier/DeleteSupplier/{Id}";
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
