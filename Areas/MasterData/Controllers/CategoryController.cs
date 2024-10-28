using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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

        private readonly IHostingEnvironment _hostingEnvironment;

        public CategoryController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            ICategoryRepository CategoryRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _categoryRepository = CategoryRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "MasterData";
            var data = _categoryRepository.GetAllCategory();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "MasterData";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _categoryRepository.GetAllCategory().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
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

        [HttpGet]
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

        [HttpGet]
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
