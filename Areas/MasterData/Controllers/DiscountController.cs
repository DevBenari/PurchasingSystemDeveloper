using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Security.Cryptography;
using System.Text;
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

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DiscountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IDiscountRepository DiscountRepository,
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
            _discountRepository = DiscountRepository;
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
                string originalPath = $"Page:MasterData/Discount/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

            var data = await _discountRepository.GetAllDiscountPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<Discount>
            {
                Items = data.discounts,
                TotalCount = data.totalCountDiscounts,
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
                string originalPath = $"Create:MasterData/Discount/CreateDiscount";
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

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:MasterData/Discount/DetailDiscount/{Id}";
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

        public IActionResult RedirectToDelete(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Delete:MasterData/Discount/DeleteDiscount/{Id}";
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
