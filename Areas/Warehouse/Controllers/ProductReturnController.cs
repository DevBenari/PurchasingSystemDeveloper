using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Repositories;
using PurchasingSystemDeveloper.Areas.Warehouse.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Route("Warehouse/[Controller]/[Action]")]
    public class ProductReturnController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductReturnRepository _productReturnRepository;
        private readonly IProductRepository _productRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IApprovalProductReturnRepository _approvalProductReturnRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;

        public ProductReturnController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IProductReturnRepository productReturnRepository,
            IProductRepository productRepository,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IApprovalProductReturnRepository approvalProductReturnRepository,
            IWarehouseLocationRepository warehouseLocationRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;
            _productReturnRepository = productReturnRepository;
            _productRepository = productRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _approvalProductReturnRepository = approvalProductReturnRepository;
            _warehouseLocationRepository = warehouseLocationRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
        }
        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:Warehouse/ProductReturn/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

        public JsonResult LoadProduk(Guid Id)
        {
            var produk = _applicationDbContext.Products.Include(p => p.Supplier).Include(s => s.Measurement).Include(s => s.WarehouseLocation).Include(d => d.Discount).Where(p => p.ProductId == Id).FirstOrDefault();
            return new JsonResult(produk);
        }

        public JsonResult LoadPosition1(Guid Id)
        {
            var position = _applicationDbContext.Positions.Where(p => p.DepartmentId == Id).ToList();
            return Json(new SelectList(position, "PositionId", "PositionName"));
        }

        public JsonResult LoadPosition2(Guid Id)
        {
            var position = _applicationDbContext.Positions.Where(p => p.DepartmentId == Id).ToList();
            return Json(new SelectList(position, "PositionId", "PositionName"));
        }

        public JsonResult LoadPosition3(Guid Id)
        {
            var position = _applicationDbContext.Positions.Where(p => p.DepartmentId == Id).ToList();
            return Json(new SelectList(position, "PositionId", "PositionName"));
        }

        public JsonResult LoadUser1(Guid Id)
        {
            var user = _applicationDbContext.UserActives.Where(p => p.PositionId == Id).ToList();
            return Json(new SelectList(user, "UserActiveId", "FullName"));
        }
        public JsonResult LoadUser2(Guid Id)
        {
            var user = _applicationDbContext.UserActives.Where(p => p.PositionId == Id).ToList();
            return Json(new SelectList(user, "UserActiveId", "FullName"));
        }
        public JsonResult LoadUser3(Guid Id)
        {
            var user = _applicationDbContext.UserActives.Where(p => p.PositionId == Id).ToList();
            return Json(new SelectList(user, "UserActiveId", "FullName"));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            ViewBag.Active = "ProductReturn";
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

            var getUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getUserLogin.KodeUser).FirstOrDefault();

            if (getUserLogin.Email == "superadmin@admin.com")
            {
                var data = await _productReturnRepository.GetAllProductReturnPageSize(searchTerm, page, pageSize, startDate, endDate);

                foreach (var item in data.ProductReturns)
                {
                    var updateData = _productReturnRepository.GetAllProductReturn().Where(u => u.ProductReturnId == item.ProductReturnId).FirstOrDefault();
                }

                var model = new Pagination<ProductReturn>
                {
                    Items = data.ProductReturns,
                    TotalCount = data.totalCountProductReturns,
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
            else
            {
                var data = await _productReturnRepository.GetAllProductReturnPageSize(searchTerm, page, pageSize, startDate, endDate);

                foreach (var item in data.ProductReturns)
                {
                    var updateData = _productReturnRepository.GetAllProductReturn().Where(u => u.ProductReturnId == item.ProductReturnId).FirstOrDefault();
                }

                var model = new Pagination<ProductReturn>
                {
                    Items = data.ProductReturns.Where(u => u.CreateBy.ToString() == getUserLogin.Id).ToList(),
                    TotalCount = data.totalCountProductReturns,
                    PageSize = pageSize,
                    CurrentPage = page,
                };

                return View(model);
            }
        }

        public IActionResult RedirectToCreate()
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Create:Warehouse/ProductReturn/CreateProductReturn";
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
        public async Task<IActionResult> CreateProductReturn()
        {
            ViewBag.Active = "ProductReturn";

            _signInManager.IsSignedIn(User);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.PurchaseOrderNumber = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);            
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);

            var productReturn = new ProductReturnViewModel()
            {
                UserAccessId = getUser.Id,
            };
            productReturn.ProductReturnDetails.Add(new ProductReturnDetail() { ProductReturnDetailId = Guid.NewGuid() });

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _productReturnRepository.GetAllProductReturn().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.ProductReturnNumber).FirstOrDefault();
            if (lastCode == null)
            {
                productReturn.ProductReturnNumber = "PRN" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.ProductReturnNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    productReturn.ProductReturnNumber = "PRN" + setDateNow + "0001";
                }
                else
                {
                    productReturn.ProductReturnNumber = "PRN" + setDateNow + (Convert.ToInt32(lastCode.ProductReturnNumber.Substring(9, lastCode.ProductReturnNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(productReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductReturn(ProductReturnViewModel model)
        {
            ViewBag.Active = "ProductReturn";

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _productReturnRepository.GetAllProductReturn().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.ProductReturnNumber).FirstOrDefault();
            if (lastCode == null)
            {
                model.ProductReturnNumber = "PRN" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.ProductReturnNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    model.ProductReturnNumber = "PRN" + setDateNow + "0001";
                }
                else
                {
                    model.ProductReturnNumber = "PRN" + setDateNow + (Convert.ToInt32(lastCode.ProductReturnNumber.Substring(9, lastCode.ProductReturnNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var prn = new ProductReturn
                {
                    CreateDateTime = DateTimeOffset.Now,
                    CreateBy = new Guid(getUser.Id), //Convert Guid to String
                    ProductReturnId = model.ProductReturnId,
                    ProductReturnNumber = model.ProductReturnNumber,
                    ReturnDate = model.ReturnDate,
                    UserAccessId = getUser.Id,
                    PurchaseOrderNumber = model.ProductReturnNumber,
                    Department1Id = model.Department1Id,
                    Position1Id = model.Position1Id,
                    UserApprove1Id = model.UserApprove1Id,
                    ApproveStatusUser1 = model.ApproveStatusUser1,
                    Department2Id = model.Department2Id,
                    Position2Id = model.Position2Id,
                    UserApprove2Id = model.UserApprove2Id,
                    ApproveStatusUser2 = model.ApproveStatusUser2,
                    Department3Id = model.Department3Id,
                    Position3Id = model.Position3Id,
                    UserApprove3Id = model.UserApprove3Id,
                    ApproveStatusUser3 = model.ApproveStatusUser3,
                    Status = model.Status,
                    ReasonForReturn = model.ReasonForReturn,
                    Note = model.Note,
                    MessageApprove1 = model.MessageApprove1,
                    MessageApprove2 = model.MessageApprove2,
                    MessageApprove3 = model.MessageApprove3
                };

                var ItemsList = new List<ProductReturnDetail>();

                foreach (var item in model.ProductReturnDetails)
                {
                    ItemsList.Add(new ProductReturnDetail
                    {
                        CreateDateTime = DateTimeOffset.Now,
                        CreateBy = new Guid(getUser.Id),
                        ProductNumber = item.ProductNumber,
                        ProductName = item.ProductName,
                        Measurement = item.Measurement,
                        WarehouseOrigin = item.WarehouseOrigin,
                        WarehouseExpired = item.WarehouseExpired,
                        Supplier = item.Supplier,
                        Qty = item.Qty,
                        Price = Math.Truncate(item.Price),
                        Discount = item.Discount,
                        SubTotal = Math.Truncate(item.SubTotal)
                    });
                }

                var getItemData = ItemsList.Where(a => a.Supplier != null).FirstOrDefault();

                prn.ProductReturnDetails = ItemsList;
                _productReturnRepository.Tambah(prn);

                ////Signal R
                //var data2 = _purchaseRequestRepository.GetAllPurchaseRequest();
                //int totalKaryawan = data2.Count();
                //await _hubContext.Clients.All.SendAsync("UpdateDataCount", totalKaryawan);
                ////End Signal R

                if (model.UserApprove1Id != null)
                {
                    var approval = new ApprovalProductReturn
                    {
                        CreateDateTime = DateTimeOffset.Now,
                        CreateBy = new Guid(getUser.Id),
                        ProductReturnId = prn.ProductReturnId,
                        ProductReturnNumber = prn.ProductReturnNumber,
                        UserAccessId = getUser.Id.ToString(),
                        UserApproveId = prn.UserApprove1Id,
                        ApproveBy = "",
                        ApprovalTime = "",
                        ApprovalDate = DateTimeOffset.MinValue,
                        ApprovalStatusUser = "User1",
                        Status = prn.Status,
                        Note = prn.Note,
                        Message = prn.MessageApprove1
                    };
                    _approvalProductReturnRepository.Tambah(approval);
                }

                if (model.UserApprove2Id != null)
                {
                    var approval = new ApprovalProductReturn
                    {
                        CreateDateTime = DateTimeOffset.Now,
                        CreateBy = new Guid(getUser.Id),
                        ProductReturnId = prn.ProductReturnId,
                        ProductReturnNumber = prn.ProductReturnNumber,
                        UserAccessId = getUser.Id.ToString(),
                        UserApproveId = prn.UserApprove2Id,
                        ApproveBy = "",
                        ApprovalTime = "",
                        ApprovalDate = DateTimeOffset.MinValue,
                        ApprovalStatusUser = "User2",
                        Status = prn.Status,
                        Note = prn.Note,
                        Message = prn.MessageApprove2
                    };
                    _approvalProductReturnRepository.Tambah(approval);
                }

                if (model.UserApprove3Id != null)
                {
                    var approval = new ApprovalProductReturn
                    {
                        CreateDateTime = DateTimeOffset.Now,
                        CreateBy = new Guid(getUser.Id),
                        ProductReturnId = prn.ProductReturnId,
                        ProductReturnNumber = prn.ProductReturnNumber,
                        UserAccessId = getUser.Id.ToString(),
                        UserApproveId = prn.UserApprove3Id,
                        ApproveBy = "",
                        ApprovalTime = "",
                        ApprovalDate = DateTimeOffset.MinValue,
                        ApprovalStatusUser = "User3",
                        Status = prn.Status,
                        Note = prn.Note,
                        Message = prn.MessageApprove3
                    };
                    _approvalProductReturnRepository.Tambah(approval);
                }

                TempData["SuccessMessage"] = "Number " + model.ProductReturnNumber + " Saved";
                return Json(new { redirectToUrl = Url.Action("Index", "ProductReturn") });
            }
            else
            {
                ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                ViewBag.PurchaseOrderNumber = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
                ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
                TempData["WarningMessage"] = "Please, input all data !";
                return View(model);
            }
        }

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:Warehouse/ProductReturn/DetailProductReturn/{Id}";
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
        public async Task<IActionResult> DetailProductReturn(Guid Id)
        {
            ViewBag.Active = "ProductReturn";

            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.PurchaseOrderNumber = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);

            var prn = await _productReturnRepository.GetProductReturnById(Id);

            if (prn == null)
            {
                Response.StatusCode = 404;
                return View("ProductReturnNotFound", Id);
            }

            var model = new ProductReturn
            {
                ProductReturnId = prn.ProductReturnId,
                ProductReturnNumber = prn.ProductReturnNumber,
                UserAccessId = prn.UserAccessId,
                Department1Id = prn.Department1Id,
                Position1Id = prn.Position1Id,
                UserApprove1Id = prn.UserApprove1Id,
                ApproveStatusUser1 = prn.ApproveStatusUser1,
                Department2Id = prn.Department2Id,
                Position2Id = prn.Position2Id,
                UserApprove2Id = prn.UserApprove2Id,
                ApproveStatusUser2 = prn.ApproveStatusUser2,
                Department3Id = prn.Department3Id,
                Position3Id = prn.Position3Id,
                UserApprove3Id = prn.UserApprove3Id,
                ApproveStatusUser3 = prn.ApproveStatusUser3,
                Status = prn.Status,               
                Note = prn.Note,
                MessageApprove1 = prn.MessageApprove1,
                MessageApprove2 = prn.MessageApprove2,
                MessageApprove3 = prn.MessageApprove3
            };

            var ItemsList = new List<ProductReturnDetail>();

            foreach (var item in prn.ProductReturnDetails)
            {
                ItemsList.Add(new ProductReturnDetail
                {
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Measurement = item.Measurement,
                    WarehouseOrigin = item.WarehouseOrigin,
                    WarehouseExpired = item.WarehouseExpired,
                    Supplier = item.Supplier,
                    Qty = item.Qty,
                    Price = Math.Truncate(item.Price),
                    Discount = item.Discount,
                    SubTotal = Math.Truncate(item.SubTotal)
                });
            }

            model.ProductReturnDetails = ItemsList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DetailProductReturn(ProductReturn model)
        {
            ViewBag.Active = "ProductReturn";

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var productReturn = _productReturnRepository.GetAllProductReturn().Where(p => p.ProductReturnNumber == model.ProductReturnNumber).FirstOrDefault();
                var approval = _approvalProductReturnRepository.GetAllApproval().Where(p => p.ProductReturnNumber == model.ProductReturnNumber).ToList();

                if (productReturn != null)
                {
                    if (approval != null)
                    {
                        foreach (var item in approval)
                        {
                            var itemApproval = approval.Where(o => o.ProductReturnId == item.ProductReturnId).FirstOrDefault();
                            if (itemApproval != null)
                            {
                                item.Note = model.Note;

                                _applicationDbContext.Entry(itemApproval).State = EntityState.Modified;
                                _applicationDbContext.SaveChanges();
                            }
                            else
                            {
                                ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                                ViewBag.PurchaseOrderNumber = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
                                ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                                ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                                ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                                ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
                                TempData["WarningMessage"] = "Name " + item.ApproveBy + " Not Found !!!";
                                return View(model);
                            }
                        }

                        productReturn.UpdateDateTime = DateTimeOffset.Now;
                        productReturn.UpdateBy = new Guid(getUser.Id);
                        productReturn.UserApprove1Id = model.UserApprove1Id;
                        productReturn.UserApprove2Id = model.UserApprove2Id;
                        productReturn.UserApprove3Id = model.UserApprove3Id;
                        productReturn.MessageApprove1 = model.MessageApprove1;
                        productReturn.MessageApprove2 = model.MessageApprove2;
                        productReturn.MessageApprove3 = model.MessageApprove3;
                        productReturn.Note = model.Note;
                        productReturn.ProductReturnDetails = model.ProductReturnDetails;

                        _productReturnRepository.Update(productReturn);

                        TempData["SuccessMessage"] = "Number " + model.ProductReturnNumber + " Changes saved";
                        return Json(new { redirectToUrl = Url.Action("Index", "ProductReturn") });
                    }
                    else
                    {
                        ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                        ViewBag.PurchaseOrderNumber = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
                        ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                        ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                        ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                        ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Number " + model.ProductReturnNumber + " Not Found !!!";
                        return View(model);
                    }
                }
                else
                {
                    ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                    ViewBag.PurchaseOrderNumber = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
                    ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                    ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                    ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                    ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Number " + model.ProductReturnNumber + " Already exists !!!";
                    return View(model);
                }
            }
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.PurchaseOrderNumber = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            TempData["WarningMessage"] = "Number " + model.ProductReturnNumber + " Failed saved";
            return Json(new { redirectToUrl = Url.Action("Index", "PurchaseRequest") });
        }
    }
}
