using FastReport.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.WebPages;
using System.Windows.Forms;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class ProductController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;

        private readonly ISupplierRepository _SupplierRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMeasurementRepository _measurementRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly HttpClient _httpClient;
        
        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IProductRepository productRepository,
            IUserActiveRepository userActiveRepository,
            
            ISupplierRepository SupplierRepository,
            ICategoryRepository categoryRepository,
            IMeasurementRepository measurementRepository,
            IDiscountRepository discountRepository,
            IWarehouseLocationRepository warehouseLocationRepository,
            HttpClient httpClient,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _productRepository = productRepository;
            _userActiveRepository = userActiveRepository;

            _SupplierRepository = SupplierRepository;
            _categoryRepository = categoryRepository;
            _measurementRepository = measurementRepository;
            _discountRepository = discountRepository;
            _warehouseLocationRepository = warehouseLocationRepository;
            _httpClient = httpClient;
            
            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> GetSuppliersPaged(string term, int page = 1, int pageSize = 10)
        {
            // Mulai query, include relasi Supplier
            var query = _applicationDbContext.Suppliers
                .AsQueryable();

            // Filter pencarian (jika user ketik di Select2)
            if (!string.IsNullOrWhiteSpace(term))
            {
                // Misal: filter by product name
                query = query.Where(p => p.SupplierName.Contains(term));
            }

            // Total data
            int totalCount = await query.CountAsync();

            // Paging
            query = query.OrderBy(p => p.SupplierName)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize);

            var items = await query.ToListAsync();

            var results = items.Select(p => new {
                id = p.SupplierId.ToString(),
                text = p.SupplierName
            });

            bool more = (page * pageSize) < totalCount;

            var response = new
            {
                results = results,
                pagination = new
                {
                    more = more
                }
            };

            return Ok(response);
        }

        public async Task<IActionResult> GetCategoriesPaged(string term, int page = 1, int pageSize = 10)
        {
            // Mulai query, include relasi Supplier
            var query = _applicationDbContext.Categories
                .AsQueryable();

            // Filter pencarian (jika user ketik di Select2)
            if (!string.IsNullOrWhiteSpace(term))
            {
                // Misal: filter by product name
                query = query.Where(p => p.CategoryName.Contains(term));
            }

            // Total data
            int totalCount = await query.CountAsync();

            // Paging
            query = query.OrderBy(p => p.CategoryName)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize);

            var items = await query.ToListAsync();

            var results = items.Select(p => new {
                id = p.CategoryId,
                text = p.CategoryName
            });

            bool more = (page * pageSize) < totalCount;

            var response = new
            {
                results = results,
                pagination = new
                {
                    more = more
                }
            };

            return Ok(response);
        }

        public async Task<IActionResult> GetMeasurementsPaged(string term, int page = 1, int pageSize = 10)
        {
            // Mulai query, include relasi Supplier
            var query = _applicationDbContext.Measurements
                .AsQueryable();

            // Filter pencarian (jika user ketik di Select2)
            if (!string.IsNullOrWhiteSpace(term))
            {
                // Misal: filter by product name
                query = query.Where(p => p.MeasurementName.Contains(term));
            }

            // Total data
            int totalCount = await query.CountAsync();

            // Paging
            query = query.OrderBy(p => p.MeasurementName)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize);

            var items = await query.ToListAsync();

            var results = items.Select(p => new {
                id = p.MeasurementId,
                text = p.MeasurementName
            });

            bool more = (page * pageSize) < totalCount;

            var response = new
            {
                results = results,
                pagination = new
                {
                    more = more
                }
            };

            return Ok(response);
        }

        public async Task<IActionResult> GetDiscountsPaged(string term, int page = 1, int pageSize = 10)
        {
            // Mulai query, include relasi Supplier
            var query = _applicationDbContext.Discounts
                .AsQueryable();

            // Filter pencarian (jika user ketik di Select2)
            if (!string.IsNullOrWhiteSpace(term))
            {
                // Misal: filter by product name
                query = query.Where(p => p.DiscountValue.ToString().Contains(term));
            }

            // Total data
            int totalCount = await query.CountAsync();

            // Paging
            query = query.OrderBy(p => p.DiscountValue)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize);

            var items = await query.ToListAsync();

            var results = items.Select(p => new {
                id = p.DiscountId,
                text = p.DiscountValue
            });

            bool more = (page * pageSize) < totalCount;

            var response = new
            {
                results = results,
                pagination = new
                {
                    more = more
                }
            };

            return Ok(response);
        }

        public async Task<IActionResult> GetWarehouseLocationsPaged(string term, int page = 1, int pageSize = 10)
        {
            // Mulai query, include relasi Supplier
            var query = _applicationDbContext.WarehouseLocations
                .AsQueryable();

            // Filter pencarian (jika user ketik di Select2)
            if (!string.IsNullOrWhiteSpace(term))
            {
                // Misal: filter by product name
                query = query.Where(p => p.WarehouseLocationName.Contains(term));
            }

            // Total data
            int totalCount = await query.CountAsync();

            // Paging
            query = query.OrderBy(p => p.WarehouseLocationName)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize);

            var items = await query.ToListAsync();

            var results = items.Select(p => new {
                id = p.WarehouseLocationId,
                text = p.WarehouseLocationName
            });

            bool more = (page * pageSize) < totalCount;

            var response = new
            {
                results = results,
                pagination = new
                {
                    more = more
                }
            };

            return Ok(response);
        }

        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:MasterData/Product/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

            var data = await _productRepository.GetAllProductPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<Product>
            {
                Items = data.products,
                TotalCount = data.totalCountProducts,
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
                string originalPath = $"Create:MasterData/Product/CreateProduct";
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
        public async Task<ViewResult> CreateProduct()
        {
            ViewBag.Active = "MasterData";            

            var product = new ProductViewModel();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _productRepository.GetAllProduct().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.ProductCode).FirstOrDefault();
            if (lastCode == null)
            {
                product.ProductCode = "PDC" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.ProductCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    product.ProductCode = "PDC" + setDateNow + "0001";
                }
                else
                {
                    product.ProductCode = "PDC" + setDateNow + (Convert.ToInt32(lastCode.ProductCode.Substring(9, lastCode.ProductCode.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _productRepository.GetAllProduct().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.ProductCode).FirstOrDefault();
            if (lastCode == null)
            {
                vm.ProductCode = "PDC" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.ProductCode.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    vm.ProductCode = "PDC" + setDateNow + "0001";
                }
                else
                {
                    vm.ProductCode = "PDC" + setDateNow + (Convert.ToInt32(lastCode.ProductCode.Substring(9, lastCode.ProductCode.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var Product = new Product
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    ProductId = vm.ProductId,
                    ProductCode = vm.ProductCode,
                    ProductName = vm.ProductName,
                    SupplierId = vm.SupplierId,
                    CategoryId = vm.CategoryId,
                    MeasurementId = vm.MeasurementId,
                    DiscountId = vm.DiscountId,
                    WarehouseLocationId = vm.WarehouseLocationId,
                    ExpiredDate = vm.ExpiredDate,
                    MinStock = vm.MinStock,
                    MaxStock = vm.MaxStock,
                    BufferStock = vm.BufferStock,
                    Stock = vm.Stock,
                    Cogs = vm.Cogs,
                    BuyPrice = vm.BuyPrice,
                    RetailPrice = vm.RetailPrice,
                    StorageLocation = vm.StorageLocation,
                    RackNumber = vm.RackNumber,
                    IsActive = true,
                    Note = vm.Note
                };

                var checkDuplicate = _productRepository.GetAllProduct().Where(c => c.ProductName == vm.ProductName).ToList();

                if (checkDuplicate.Count == 0)
                {
                    var result = _productRepository.GetAllProduct().Where(c => c.ProductName == vm.ProductName).FirstOrDefault();
                    if (result == null)
                    {
                        _productRepository.Tambah(Product);
                        TempData["SuccessMessage"] = "Name " + vm.ProductName + " Saved";
                        return RedirectToAction("Index", "Product");
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Name " + vm.ProductName + " Already Exist !!!";
                        return View(vm);
                    }
                } 
                else 
                {
                    TempData["WarningMessage"] = "Name " + vm.ProductName + " There is duplicate data !!!";
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
                string originalPath = $"Detail:MasterData/Product/DetailProduct/{Id}";
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
        public async Task<IActionResult> DetailProduct(Guid Id)
        {
            ViewBag.Active = "MasterData";            

            var Product = await _productRepository.GetProductById(Id);            

            if (Product == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", Id);
            }

            ProductViewModel viewModel = new ProductViewModel
            {
                ProductId = Product.ProductId,
                ProductCode = Product.ProductCode,
                ProductName = Product.ProductName,
                SupplierId = Product.SupplierId,
                SupplierName = Product.Supplier.SupplierName,
                CategoryId = Product.CategoryId,
                CategoryName = Product.Category.CategoryName,
                MeasurementId = Product.MeasurementId,
                MeasurementName = Product.Measurement.MeasurementName,
                DiscountId = Product.DiscountId,
                DiscountValue = Product.Discount.DiscountValue,
                WarehouseLocationId = Product.WarehouseLocationId,
                WarehouseLocationName = Product.WarehouseLocation.WarehouseLocationName,
                ExpiredDate = Product.ExpiredDate,
                MinStock = Product.MinStock,
                MaxStock = Product.MaxStock,
                BufferStock = Product.BufferStock,
                Stock = Product.Stock,
                Cogs = Math.Truncate((decimal)Product.Cogs),
                BuyPrice = Math.Truncate(Product.BuyPrice),
                RetailPrice = Math.Truncate(Product.RetailPrice),
                StorageLocation = Product.StorageLocation,
                RackNumber = Product.RackNumber,
                IsActive = Product.IsActive,
                Note = Product.Note                
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailProduct(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Product = await _productRepository.GetProductByIdNoTracking(viewModel.ProductId);
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var checkDuplicate = _productRepository.GetAllProduct().Where(d => d.ProductName == viewModel.ProductName && d.SupplierId == viewModel.SupplierId).ToList();

                if (checkDuplicate.Count == 0 || checkDuplicate.Count == 1)
                {
                    var data = _productRepository.GetAllProduct().Where(d => d.ProductCode == viewModel.ProductCode).FirstOrDefault();

                    if (data != null)
                    {
                        Product.UpdateDateTime = DateTime.Now;
                        Product.UpdateBy = new Guid(getUser.Id);
                        Product.ProductCode = viewModel.ProductCode;
                        Product.ProductName = viewModel.ProductName;
                        Product.SupplierId = viewModel.SupplierId;
                        Product.CategoryId = viewModel.CategoryId;
                        Product.MeasurementId = viewModel.MeasurementId;
                        Product.DiscountId = viewModel.DiscountId;
                        Product.WarehouseLocationId = viewModel.WarehouseLocationId;
                        Product.ExpiredDate = viewModel.ExpiredDate;
                        Product.MinStock = viewModel.MinStock;
                        Product.MaxStock = viewModel.MaxStock;
                        Product.BufferStock = viewModel.BufferStock;
                        Product.Stock = viewModel.Stock;
                        Product.Cogs = viewModel.Cogs;
                        Product.BuyPrice = viewModel.BuyPrice;
                        Product.RetailPrice = viewModel.RetailPrice;
                        Product.StorageLocation = viewModel.StorageLocation;
                        Product.RackNumber = viewModel.RackNumber;
                        Product.IsActive = viewModel.IsActive;
                        Product.Note = viewModel.Note;

                        _productRepository.Update(Product);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Name " + viewModel.ProductName + " Success Changes";
                        return RedirectToAction("Index", "Product");
                    }
                    else
                    {
                        await FillDataVM(viewModel);
                        TempData["WarningMessage"] = "Name " + viewModel.ProductName + " Already Exist !!!";
                        return View(viewModel);
                    }
                } 
                else
                {
                    await FillDataVM(viewModel);
                    TempData["WarningMessage"] = "Name " + viewModel.ProductName + " There is duplicate data !!!";
                    return View(viewModel);
                }
            }
            await FillDataVM(viewModel);
            return View(viewModel);
        }

        public IActionResult RedirectToDelete(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Delete:MasterData/Product/DeleteProduct/{Id}";
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
        public async Task<IActionResult> DeleteProduct(Guid Id)
        {
            ViewBag.Active = "MasterData";
            var Product = await _productRepository.GetProductById(Id);
            if (Product == null)
            {
                Response.StatusCode = 404;
                return View("ProductNotFound", Id);
            }

            ProductViewModel vm = new ProductViewModel
            {
                ProductId = Product.ProductId,
                ProductCode = Product.ProductCode,
                ProductName = Product.ProductName,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(ProductViewModel vm)
        {
            //Hapus Data
            var Product = _applicationDbContext.Products.FirstOrDefault(x => x.ProductId == vm.ProductId);
            _applicationDbContext.Attach(Product);
            _applicationDbContext.Entry(Product).State = EntityState.Deleted;
            _applicationDbContext.SaveChanges();

            TempData["SuccessMessage"] = "Name " + vm.ProductName + " Success Deleted";
            return RedirectToAction("Index", "Product");
        }

        private async Task FillDataVM(ProductViewModel viewModel)
        {
            // Supplier
            if (viewModel.SupplierId != Guid.Empty)
            {
                var supplier = await _applicationDbContext.Suppliers.FindAsync(viewModel.SupplierId);
                if (supplier != null)
                {
                    viewModel.SupplierName = supplier.SupplierName;
                }
            }

            // Category
            if (viewModel.CategoryId != Guid.Empty)
            {
                var category = await _applicationDbContext.Categories.FindAsync(viewModel.CategoryId);
                if (category != null)
                {
                    viewModel.CategoryName = category.CategoryName;
                }
            }

            // Measurement
            if (viewModel.MeasurementId != Guid.Empty)
            {
                var measure = await _applicationDbContext.Measurements.FindAsync(viewModel.MeasurementId);
                if (measure != null)
                {
                    viewModel.MeasurementName = measure.MeasurementName;
                }
            }

            // Discount
            if (viewModel.DiscountId != Guid.Empty)
            {
                var discount = await _applicationDbContext.Discounts.FindAsync(viewModel.DiscountId);
                if (discount != null)
                {
                    viewModel.DiscountValue = discount.DiscountValue;
                }
            }

            // Warehouse Location
            if (viewModel.WarehouseLocationId != Guid.Empty)
            {
                var warehouse = await _applicationDbContext.WarehouseLocations.FindAsync(viewModel.WarehouseLocationId);
                if (warehouse != null)
                {
                    viewModel.WarehouseLocationName = warehouse.WarehouseLocationName;
                }
            }
        }
    }
}
