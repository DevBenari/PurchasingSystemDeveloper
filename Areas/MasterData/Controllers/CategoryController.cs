using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.DataProtection;
using PurchasingSystemDeveloper.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class CategoryController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CategoryController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            ICategoryRepository CategoryRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _categoryRepository = CategoryRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;

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
                string originalPath = $"Page:MasterData/Category/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

            var data = await _categoryRepository.GetAllCategoryPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<Category>
            {
                Items = data.categories,
                TotalCount = data.totalCountCategories,
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
                string originalPath = $"Create:MasterData/Category/CreateCategory";
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
        
        public async Task<ViewResult> CreateCategory()
        {
            ViewBag.Active = "MasterData";
            var user = new CategoryViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _categoryRepository.GetAllCategory().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.CategoryCode).FirstOrDefault();
            if (lastCode == null)
            {
                user.CategoryCode = "CTG" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.CategoryCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    user.CategoryCode = "CTG" + setDateNow + "0001";
                }
                else
                {
                    user.CategoryCode = "CTG" + setDateNow + (Convert.ToInt32(lastCode.CategoryCode.Substring(9, lastCode.CategoryCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _categoryRepository.GetAllCategory().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.CategoryCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.CategoryCode = "CTG" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.CategoryCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.CategoryCode = "CTG" + setDateNow + "0001";
                }
                else
                {
                    vm.CategoryCode = "CTG" + setDateNow + (Convert.ToInt32(lastCode.CategoryCode.Substring(9, lastCode.CategoryCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var Category = new Category
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    CategoryId = vm.CategoryId,
                    CategoryCode = vm.CategoryCode,
                    CategoryName = vm.CategoryName,
                    Note = vm.Note
                };

                var checkDuplicate = _categoryRepository.GetAllCategory().Where(c => c.CategoryName == vm.CategoryName).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _categoryRepository.GetAllCategory().Where(c => c.CategoryName == vm.CategoryName).FirstOrDefault();
                    if (result == null)
                    {
                        _categoryRepository.Tambah(Category);
                        TempData["SuccessMessage"] = "Name " + vm.CategoryName + " Saved";
                        return RedirectToAction("Index", "Category");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Name " + vm.CategoryName + " Already Exist !!!";
                        return View(vm);
                    }
                }
                else 
                {
                    TempData["WarningMessage"] = "Name " + vm.CategoryName + " There is duplicate data !!!";
                    return View(vm);
                }
            }
            else
            {                
                return View(vm);
            }
        }

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:MasterData/Category/DetailCategory/{Id}";
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
        
        public async Task<IActionResult> DetailCategory(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var Category = await _categoryRepository.GetCategoryById(Id);

            if (Category == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            CategoryViewModel viewModel = new CategoryViewModel
            {
                CategoryId = Category.CategoryId,
                CategoryCode = Category.CategoryCode,
                CategoryName = Category.CategoryName,
                Note = Category.Note
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailCategory(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Category = await _categoryRepository.GetCategoryByIdNoTracking(viewModel.CategoryId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var checkDuplicate = _categoryRepository.GetAllCategory().Where(d => d.CategoryName == viewModel.CategoryName).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var data = _categoryRepository.GetAllCategory().Where(d => d.CategoryCode == viewModel.CategoryCode).FirstOrDefault();

                    if (data != null)
                    {
                        Category.UpdateDateTime = DateTime.Now;
                        Category.UpdateBy = new Guid(getUser.Id);
                        Category.CategoryCode = viewModel.CategoryCode;
                        Category.CategoryName = viewModel.CategoryName;
                        Category.Note = viewModel.Note;

                        _categoryRepository.Update(Category);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Name " + viewModel.CategoryName + " Success Changes";
                        return RedirectToAction("Index", "Category");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Name " + viewModel.CategoryName + " Already Exist !!!";
                        return View(viewModel);
                    }
                }
                else 
                {
                    TempData["WarningMessage"] = "Name " + viewModel.CategoryName + " There is duplicate data !!!";
                    return View(viewModel);
                }
            }
            else
            {
                return View(viewModel);
            }            
        }

        public IActionResult RedirectToDelete(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Delete:MasterData/Category/DeleteCategory/{Id}";
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
        
        public async Task<IActionResult> DeleteCategory(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var Category = await _categoryRepository.GetCategoryById(Id);
            if (Category == null)
            {
                Response.StatusCode = 404;
                return View("CategoryNotFound", Id);
            }

            CategoryViewModel vm = new CategoryViewModel
            {
                CategoryId = Category.CategoryId,
                CategoryCode = Category.CategoryCode,
                CategoryName = Category.CategoryName,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(CategoryViewModel vm)
        {
            //Cek Relasi Principal dengan Produk
            var produk = _productRepository.GetAllProduct().Where(p => p.CategoryId == vm.CategoryId).FirstOrDefault();
            if (produk == null)
            {
                //Hapus Data
                var Category = _applicationDbContext.Categories.FirstOrDefault(x => x.CategoryId == vm.CategoryId);
                _applicationDbContext.Attach(Category);
                _applicationDbContext.Entry(Category).State = EntityState.Deleted;
                _applicationDbContext.SaveChanges();

                TempData["SuccessMessage"] = "Name " + vm.CategoryName + " Success Deleted";
                return RedirectToAction("Index", "Category");
            }
            else {
                TempData["WarningMessage"] = "Sorry, " + vm.CategoryName + " In used by the product !";
                return View(vm);
            }                
        }
    }
}
