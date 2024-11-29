using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class UserActiveController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly ILogger<UserActiveController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UserActiveController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IWarehouseLocationRepository warehouseLocationRepository,
            IDepartmentRepository departmentRepository, 
            IPositionRepository positionRepository,
            IPurchaseRequestRepository purchaseRequestRepository,
            IPurchaseOrderRepository purchaseOrderRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            ILogger<UserActiveController> logger,
            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;
            _warehouseLocationRepository = warehouseLocationRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _purchaseOrderRepository = purchaseOrderRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
            _logger = logger;   
            _hostingEnvironment = hostingEnvironment;
        }

        public JsonResult LoadPosition(Guid Id)
        {
            var position = _applicationDbContext.Positions.Where(p => p.DepartmentId == Id).ToList();
            return Json(new SelectList(position, "PositionId", "PositionName"));
        }

        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:MasterData/UserActive/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

            var data = await _userActiveRepository.GetAllUserActivePageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<UserActive>
            {
                Items = data.userActives,
                TotalCount = data.totalCountUserActives,
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
                string originalPath = $"Create:MasterData/UserActive/CreateUserActive";
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
        public async Task<ViewResult> CreateUserActive()
        {
            ViewBag.Active = "MasterData";

            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);

            var user = new UserActiveViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _userActiveRepository.GetAllUser().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.UserActiveCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.UserActiveCode = "USR" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.UserActiveCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.UserActiveCode = "USR" + setDateNow + "0001";
                }
                else
                {
                    user.UserActiveCode = "USR" + setDateNow + (Convert.ToInt32(lastCode.UserActiveCode.Substring(9, lastCode.UserActiveCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserActive(UserActiveViewModel vm)
        {
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _userActiveRepository.GetAllUser().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.UserActiveCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.UserActiveCode = "USR" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.UserActiveCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.UserActiveCode = "USR" + setDateNow + "0001";
                }
                else
                {
                    vm.UserActiveCode = "USR" + setDateNow + (Convert.ToInt32(lastCode.UserActiveCode.Substring(9, lastCode.UserActiveCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {

                if(vm.Foto != null)
                {
                    long fileSize = vm.Foto.Length;
                    long maxSize = 2 * 1024 * 1024;

                    if(fileSize> maxSize)
                    {
                        TempData["ErrorMessage"] = "Ukuran file tidak boleh lebih dari 2Mb";
                        ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                        ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                        return View(vm);
                    }
                }

                string uniqueFileName = ProcessUploadFile(vm);

                var userLogin = new ApplicationUser
                {
                    KodeUser = vm.UserActiveCode,
                    NamaUser = vm.FullName,
                    Email = vm.Email,
                    UserName = vm.Email,
                    IsActive = true
                };

                var user = new UserActive
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    UserActiveId = vm.UserActiveId,
                    UserActiveCode = vm.UserActiveCode,
                    FullName = vm.FullName,
                    IdentityNumber = vm.IdentityNumber,
                    DepartmentId = vm.DepartmentId,
                    PositionId = vm.PositionId,
                    PlaceOfBirth = vm.PlaceOfBirth,
                    DateOfBirth = vm.DateOfBirth,
                    Gender = vm.Gender,
                    Address = vm.Address,
                    Handphone = vm.Handphone,
                    Email = vm.Email,
                    Foto = uniqueFileName,
                    IsActive = true
                };

                var passTglLahir = vm.DateOfBirth.ToString("ddMMMyyyy");                
                var resultLogin = await _userManager.CreateAsync(userLogin, passTglLahir);

                _logger.LogInformation($"add a user to the create user page on : {DateTime.Now.TimeOfDay}");

                var checkDuplicatePengguna = _userActiveRepository.GetAllUser().Where(c => c.FullName == vm.FullName && c.DepartmentId == vm.DepartmentId).ToList();

                if (checkDuplicatePengguna.Count == 0)
                {
                    var resultPengguna = _userActiveRepository.GetAllUser().Where(c => c.FullName == vm.FullName).FirstOrDefault();

                    if (resultPengguna == null)
                    {
                        if (resultLogin.Succeeded)
                        {
                            _userActiveRepository.Tambah(user);
                            TempData["SuccessMessage"] = "Account " + vm.FullName + " Saved";
                            return RedirectToAction("Index", "UserActive");
                        }
                        else
                        {
                            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                            TempData["WarningMessage"] = "Account " + vm.FullName + " Already Exist !!!";
                            return View(vm);
                        }
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                        ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Account " + vm.FullName + " Already Exist !!!";
                        return View(vm);
                    }
                }
                else
                {
                    ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                    ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Account " + vm.FullName + " There is duplicate data !!!";
                    return View(vm);
                }
            }
            else
            {
                ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);                
                return View(vm);
            }
        }

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:MasterData/UserActive/DetailUserActive/{Id}";
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
        public async Task<IActionResult> DetailUserActive(Guid Id)
        {
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);

            ViewBag.Active = "MasterData";
            var user = await _userActiveRepository.GetUserById(Id);

            if (user == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            UserActiveViewModel viewModel = new UserActiveViewModel
            {
                UserActiveId = user.UserActiveId,
                UserActiveCode = user.UserActiveCode,
                FullName = user.FullName,
                IdentityNumber = user.IdentityNumber,
                DepartmentId = user.DepartmentId,
                PositionId = user.PositionId,
                PlaceOfBirth = user.PlaceOfBirth,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Address = user.Address,
                Handphone = user.Handphone,
                Email = user.Email,
                UserPhotoPath = user.Foto,
                IsActive = user.IsActive
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailUserActive(UserActiveViewModel viewModel)
        {
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);

            if (ModelState.IsValid)
            {
                var user = await _userActiveRepository.GetUserByIdNoTracking(viewModel.UserActiveId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var userLogin = await _userManager.FindByNameAsync(viewModel.Email);
                var checkDuplicate = _userActiveRepository.GetAllUser().Where(d => d.UserActiveCode == viewModel.UserActiveCode).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var data = _userActiveRepository.GetAllUser().Where(d => d.UserActiveCode == viewModel.UserActiveCode).FirstOrDefault();

                    if (data != null)
                    {
                        if (userLogin != null)
                        {
                            user.UpdateDateTime = DateTime.Now;
                            user.UpdateBy = new Guid(getUser.Id);
                            user.UserActiveCode = viewModel.UserActiveCode;
                            user.FullName = viewModel.FullName;
                            user.IdentityNumber = viewModel.IdentityNumber;
                            user.DepartmentId = viewModel.DepartmentId;
                            user.PositionId = viewModel.PositionId;
                            user.PlaceOfBirth = viewModel.PlaceOfBirth;
                            user.DateOfBirth = viewModel.DateOfBirth;
                            user.Gender = viewModel.Gender;
                            user.Address = viewModel.Address;
                            user.Handphone = viewModel.Handphone;
                            user.Email = viewModel.Email;
                            user.IsActive = viewModel.IsActive;

                            if (viewModel.Foto != null)
                            {
                                if (viewModel.UserPhotoPath != null)
                                {
                                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                                        "UserPhoto", viewModel.UserPhotoPath);
                                    System.IO.File.Delete(filePath);
                                }

                                if (viewModel.Foto != null)
                                {
                                    long fileSize = viewModel.Foto.Length;
                                    long maxSize = 2 * 1024 * 1024;

                                    if (fileSize > maxSize)
                                    {
                                        TempData["ErrorMessage"] = "Ukuran file tidak boleh lebih dari 2Mb";
                                        ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                                        ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                                        return View(viewModel);
                                    }
                                }

                                user.Foto = ProcessUploadFile(viewModel);
                            }

                            userLogin.NamaUser = viewModel.FullName;
                            userLogin.UserName = viewModel.Email;
                            userLogin.Email = viewModel.Email;
                            userLogin.IsActive = viewModel.IsActive;

                            var result = await _userManager.UpdateAsync(userLogin);

                            if (result.Succeeded)
                            {
                                _userActiveRepository.Update(user);
                                _applicationDbContext.SaveChanges();

                                TempData["SuccessMessage"] = "Account " + viewModel.FullName + " Success Changes";
                                return RedirectToAction("Index", "UserActive");
                            }
                        }
                        else
                        {
                            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                            TempData["WarningMessage"] = "Account " + viewModel.FullName + " Sorry, Data Failed !!!";
                            return View(viewModel);
                        }
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                        ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Account " + viewModel.FullName + " Already Exist !!!";
                        return View(viewModel);
                    }
                }
                else
                {
                    ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                    ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Account " + viewModel.FullName + " There is duplicate data !!!";
                    return View(viewModel);
                }
            }
            else
            {
                ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);                
                return View(viewModel);
            }

            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
            return View(viewModel);
        }

        public IActionResult RedirectToDelete(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Delete:MasterData/UserActive/DeleteUserActive/{Id}";
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
        public async Task<IActionResult> DeleteUserActive(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var user = await _userActiveRepository.GetUserById(Id);
            if (user == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            UserActiveViewModel vm = new UserActiveViewModel
            {
                UserActiveId = user.UserActiveId,
                UserActiveCode = user.UserActiveCode,
                FullName = user.FullName,
                UserPhotoPath = user.Foto,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserActive(UserActiveViewModel vm)
        {
            //Cek Relasi
            var userPurchaseRequest = _purchaseRequestRepository.GetAllPurchaseRequest().Where(p => p.UserAccessId == vm.UserActiveId.ToString()).FirstOrDefault();
            var userPurchaseOrder = _purchaseOrderRepository.GetAllPurchaseOrder().Where(p => p.UserAccessId == vm.UserActiveId.ToString()).FirstOrDefault();
            var userWarehouse = _warehouseLocationRepository.GetAllWarehouseLocation().Where(p => p.WarehouseManagerId == vm.UserActiveId).FirstOrDefault();
            
            if (userWarehouse == null)
            {
                //Hapus Akun Login
                var userLogin = _signInManager.UserManager.Users.FirstOrDefault(s => s.KodeUser == vm.UserActiveCode);
                _applicationDbContext.Attach(userLogin);
                _applicationDbContext.Entry(userLogin).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                //Hapus Data Profil
                var userActive = _applicationDbContext.UserActives.FirstOrDefault(x => x.UserActiveId == vm.UserActiveId);
                _applicationDbContext.Attach(userActive);
                _applicationDbContext.Entry(userActive).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                if (vm.UserPhotoPath != null)
                {
                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                        "UserPhoto", vm.UserPhotoPath);
                    System.IO.File.Delete(filePath);
                }

                userActive.Foto = ProcessUploadFile(vm);

                TempData["SuccessMessage"] = "Account " + vm.FullName + " Success Deleted";

                return RedirectToAction("Index", "UserActive");
            }
            else if (userPurchaseRequest == null)
            {
                //Hapus Akun Login
                var userLogin = _signInManager.UserManager.Users.FirstOrDefault(s => s.KodeUser == vm.UserActiveCode);
                _applicationDbContext.Attach(userLogin);
                _applicationDbContext.Entry(userLogin).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                //Hapus Data Profil
                var userActive = _applicationDbContext.UserActives.FirstOrDefault(x => x.UserActiveId == vm.UserActiveId);
                _applicationDbContext.Attach(userActive);
                _applicationDbContext.Entry(userActive).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                if (vm.UserPhotoPath != null)
                {
                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                        "UserPhoto", vm.UserPhotoPath);
                    System.IO.File.Delete(filePath);
                }

                userActive.Foto = ProcessUploadFile(vm);

                TempData["SuccessMessage"] = "Account " + vm.FullName + " Success Deleted";

                return RedirectToAction("Index", "UserActive");
            }
            else if (userPurchaseOrder == null)
            {
                //Hapus Akun Login
                var userLogin = _signInManager.UserManager.Users.FirstOrDefault(s => s.KodeUser == vm.UserActiveCode);
                _applicationDbContext.Attach(userLogin);
                _applicationDbContext.Entry(userLogin).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                //Hapus Data Profil
                var userActive = _applicationDbContext.UserActives.FirstOrDefault(x => x.UserActiveId == vm.UserActiveId);
                _applicationDbContext.Attach(userActive);
                _applicationDbContext.Entry(userActive).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                if (vm.UserPhotoPath != null)
                {
                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                        "UserPhoto", vm.UserPhotoPath);
                    System.IO.File.Delete(filePath);
                }

                userActive.Foto = ProcessUploadFile(vm);

                TempData["SuccessMessage"] = "Account " + vm.FullName + " Success Deleted";

                return RedirectToAction("Index", "UserActive");
            }
            else
            {
                TempData["WarningMessage"] = "Sorry, " + vm.FullName + " In used by the warehouse location !";
                return View(vm);
            }
        }
        private string ProcessUploadFile(UserActiveViewModel model)
        {
            
            if(model.Foto == null)
            {
                return null;
            }

            string[] FileAkses = { ".jpg", ".jepg" ,".png", ".gif"};
            string fileExtensions = Path.GetExtension(model.Foto.FileName).ToLowerInvariant();

            if(!FileAkses.Contains(fileExtensions))
                {
                    throw new InvalidOperationException("format file harus png, jpg, jepg, gif");
                }

            if(model.Foto.Length > 2 * 1024 * 1024)
                {
                    throw new InvalidOperationException("ukuran file telah melebihi batas maksimum 2MB");
                }

            string safeFileName = Path.GetFileNameWithoutExtension(model.Foto.FileName);
            safeFileName = Regex.Replace(safeFileName, @"[^a-zA-Z0-9-_]", "");

                var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}_{DateTime.Now.Ticks}{fileExtensions}";
                string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "UserPhoto");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Foto.CopyTo(fileStream);
                }
            return uniqueFileName;
        }
    }
}
