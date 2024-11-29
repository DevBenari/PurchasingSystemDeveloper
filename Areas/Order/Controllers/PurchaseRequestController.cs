using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Order.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Hubs;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("Order/[Controller]/[Action]")]
    public class PurchaseRequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly ILeadTimeRepository _leadTimeRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;


        public PurchaseRequestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IPurchaseRequestRepository purchaseRequestRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IApprovalRepository approvalRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository,
            ILeadTimeRepository leadTimeRepository,
            ISupplierRepository supplierRepository,
            IHubContext<ChatHub> hubContext,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _purchaseRequestRepository = purchaseRequestRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _approvalRepository = approvalRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _leadTimeRepository = leadTimeRepository;
            _supplierRepository = supplierRepository;
            _hubContext = hubContext;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
            _hostingEnvironment = hostingEnvironment;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public JsonResult LoadProduk(Guid Id)
        {
            var produk = _applicationDbContext.Products.Include(p => p.Supplier).Include(s => s.Measurement).Include(d => d.Discount).Where(p => p.ProductId == Id).FirstOrDefault();
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

        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:Order/PurchaseRequest/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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
            ViewBag.Active = "PurchaseRequest";
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
                var data = await _purchaseRequestRepository.GetAllPurchaseRequestPageSize(searchTerm, page, pageSize, startDate, endDate);

                foreach (var item in data.purchaseRequests)
                {
                    var remainingDay = DateTimeOffset.Now.Date - item.CreateDateTime.Date;
                    var updateData = _purchaseRequestRepository.GetAllPurchaseRequest().Where(u => u.PurchaseRequestId == item.PurchaseRequestId).FirstOrDefault();

                    if (updateData.RemainingDay != 0)
                    {
                        updateData.RemainingDay = item.ExpiredDay - remainingDay.Days;

                        _applicationDbContext.PurchaseRequests.Update(updateData);
                        _applicationDbContext.SaveChanges();
                    }
                }

                var model = new Pagination<PurchaseRequest>
                {
                    Items = data.purchaseRequests,
                    TotalCount = data.totalCountPurchaseRequests,
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
                var data = await _purchaseRequestRepository.GetAllPurchaseRequestPageSize(searchTerm, page, pageSize, startDate, endDate);

                foreach (var item in data.purchaseRequests)
                {
                    var remainingDay = DateTimeOffset.Now.Date - item.CreateDateTime.Date;
                    var updateData = _purchaseRequestRepository.GetAllPurchaseRequest().Where(u => u.PurchaseRequestId == item.PurchaseRequestId).FirstOrDefault();

                    if (updateData.RemainingDay != 0)
                    {
                        updateData.RemainingDay = item.ExpiredDay - remainingDay.Days;

                        _applicationDbContext.PurchaseRequests.Update(updateData);
                        _applicationDbContext.SaveChanges();
                    }                        
                }

                var model = new Pagination<PurchaseRequest>
                {
                    Items = data.purchaseRequests.Where(u => u.CreateBy.ToString() == getUserLogin.Id).ToList(),
                    TotalCount = data.totalCountPurchaseRequests,
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
                string originalPath = $"Create:Order/PurchaseRequest/CreatePurchaseRequest";
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
        public async Task<IActionResult> CreatePurchaseRequest(string searchTerm)
        {
            ViewBag.Active = "PurchaseRequest";

            _signInManager.IsSignedIn(User);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
           
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            //ViewBag.Product = from p in _applicationDbContext.Products.Include(s => s.Supplier).ToList() select new { ProductId = p.ProductId, ProductName = p.ProductName, Supplier = p.Supplier.SupplierName };            
            ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);            
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);

            PurchaseRequest purchaseRequest = new PurchaseRequest()
            {
                UserAccessId = getUser.Id,
            };
            purchaseRequest.PurchaseRequestDetails.Add(new PurchaseRequestDetail() { PurchaseRequestDetailId = Guid.NewGuid() });

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _purchaseRequestRepository.GetAllPurchaseRequest().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.PurchaseRequestNumber).FirstOrDefault();
            if (lastCode == null)
            {
                purchaseRequest.PurchaseRequestNumber = "PR" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.PurchaseRequestNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    purchaseRequest.PurchaseRequestNumber = "PR" + setDateNow + "0001";
                }
                else
                {
                    purchaseRequest.PurchaseRequestNumber = "PR" + setDateNow + (Convert.ToInt32(lastCode.PurchaseRequestNumber.Substring(9, lastCode.PurchaseRequestNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(purchaseRequest);
        }        

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseRequest(PurchaseRequest model)
        {
            ViewBag.Active = "PurchaseRequest";

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _purchaseRequestRepository.GetAllPurchaseRequest().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.PurchaseRequestNumber).FirstOrDefault();
            if (lastCode == null)
            {
                model.PurchaseRequestNumber = "PR" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.PurchaseRequestNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    model.PurchaseRequestNumber = "PR" + setDateNow + "0001";
                }
                else
                {
                    model.PurchaseRequestNumber = "PR" + setDateNow + (Convert.ToInt32(lastCode.PurchaseRequestNumber.Substring(9, lastCode.PurchaseRequestNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();            

            if (ModelState.IsValid)
            {
                var purchaseRequest = new PurchaseRequest
                {
                    CreateDateTime = DateTimeOffset.Now,
                    CreateBy = new Guid(getUser.Id), //Convert Guid to String
                    PurchaseRequestId = model.PurchaseRequestId,
                    PurchaseRequestNumber = model.PurchaseRequestNumber,
                    UserAccessId = getUser.Id,
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
                    TermOfPaymentId = model.TermOfPaymentId,
                    Status = model.Status,
                    QtyTotal = model.QtyTotal,
                    GrandTotal = Math.Truncate(model.GrandTotal),
                    Note = model.Note,
                    MessageApprove1 = model.MessageApprove1,
                    MessageApprove2 = model.MessageApprove2,
                    MessageApprove3 = model.MessageApprove3
                };                

                var ItemsList = new List<PurchaseRequestDetail>();

                foreach (var item in model.PurchaseRequestDetails)
                {
                    ItemsList.Add(new PurchaseRequestDetail
                    {
                        CreateDateTime = DateTimeOffset.Now,
                        CreateBy = new Guid(getUser.Id),
                        ProductNumber = item.ProductNumber,
                        ProductName = item.ProductName,
                        Measurement = item.Measurement,
                        Supplier = item.Supplier,
                        Qty = item.Qty,
                        Price = Math.Truncate(item.Price),
                        Discount = item.Discount,
                        SubTotal = Math.Truncate(item.SubTotal)
                    });
                }

                var getItemData = ItemsList.Where(a => a.Supplier != null).FirstOrDefault();

                var getSupplier = _supplierRepository.GetAllSupplier().Where(s => s.SupplierName == getItemData.Supplier).FirstOrDefault();

                var getLeadTime = getSupplier.LeadTime.LeadTimeValue;

                var expiredDay = DateTimeOffset.Now.Date.AddDays(Convert.ToDouble(getLeadTime));

                var remainingDay = DateTimeOffset.Now.Date - purchaseRequest.CreateDateTime.Date;

                purchaseRequest.ExpiredDay = getLeadTime;
                purchaseRequest.ExpiredDate = expiredDay;
                purchaseRequest.RemainingDay = purchaseRequest.ExpiredDay - remainingDay.Days;

                purchaseRequest.PurchaseRequestDetails = ItemsList;
                _purchaseRequestRepository.Tambah(purchaseRequest);

                //Signal R
                var data2 = _purchaseRequestRepository.GetAllPurchaseRequest();
                int totalKaryawan = data2.Count();
                await _hubContext.Clients.All.SendAsync("UpdateDataCount", totalKaryawan);
                //End Signal R                

                if (model.UserApprove1Id != null) 
                {
                    var approval = new Approval
                    {
                        CreateDateTime = DateTimeOffset.Now,
                        CreateBy = new Guid(getUser.Id),
                        PurchaseRequestId = purchaseRequest.PurchaseRequestId,
                        PurchaseRequestNumber = purchaseRequest.PurchaseRequestNumber,
                        UserAccessId = getUser.Id.ToString(),
                        ExpiredDay = purchaseRequest.ExpiredDay,
                        RemainingDay = purchaseRequest.RemainingDay,
                        ExpiredDate = purchaseRequest.ExpiredDate,
                        UserApproveId = purchaseRequest.UserApprove1Id,
                        ApproveBy = "",
                        ApprovalTime = "",
                        ApprovalDate = DateTimeOffset.MinValue,
                        ApprovalStatusUser = "User1",
                        Status = purchaseRequest.Status,
                        Note = purchaseRequest.Note,
                        Message = purchaseRequest.MessageApprove1
                    };
                    _approvalRepository.Tambah(approval);
                }

                if (model.UserApprove2Id != null)
                {
                    var approval = new Approval
                    {
                        CreateDateTime = DateTimeOffset.Now,
                        CreateBy = new Guid(getUser.Id),
                        PurchaseRequestId = purchaseRequest.PurchaseRequestId,
                        PurchaseRequestNumber = purchaseRequest.PurchaseRequestNumber,
                        UserAccessId = getUser.Id.ToString(),
                        ExpiredDay = purchaseRequest.ExpiredDay,
                        RemainingDay = purchaseRequest.RemainingDay,
                        ExpiredDate = purchaseRequest.ExpiredDate,
                        UserApproveId = purchaseRequest.UserApprove2Id,
                        ApproveBy = "",
                        ApprovalTime = "",
                        ApprovalDate = DateTimeOffset.MinValue,
                        ApprovalStatusUser = "User2",
                        Status = purchaseRequest.Status,
                        Note = purchaseRequest.Note,
                        Message = purchaseRequest.MessageApprove2
                    };
                    _approvalRepository.Tambah(approval);
                }

                if (model.UserApprove3Id != null)
                {
                    var approval = new Approval
                    {
                        CreateDateTime = DateTimeOffset.Now,
                        CreateBy = new Guid(getUser.Id),
                        PurchaseRequestId = purchaseRequest.PurchaseRequestId,
                        PurchaseRequestNumber = purchaseRequest.PurchaseRequestNumber,
                        UserAccessId = getUser.Id.ToString(),
                        ExpiredDay = purchaseRequest.ExpiredDay,
                        RemainingDay = purchaseRequest.RemainingDay,
                        ExpiredDate = purchaseRequest.ExpiredDate,
                        UserApproveId = purchaseRequest.UserApprove3Id,
                        ApproveBy = "",
                        ApprovalTime = "",
                        ApprovalDate = DateTimeOffset.MinValue,
                        ApprovalStatusUser = "User3",
                        Status = purchaseRequest.Status,
                        Note = purchaseRequest.Note,
                        Message = purchaseRequest.MessageApprove3
                    };
                    _approvalRepository.Tambah(approval);
                }

                TempData["SuccessMessage"] = "Number " + model.PurchaseRequestNumber + " Saved";
                return Json(new { redirectToUrl = Url.Action("Index", "PurchaseRequest") });
            }
            else
            {
                ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);                
                ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                TempData["WarningMessage"] = "Please, input all data !";
                return View(model);
            }
        }

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:Order/PurchaseRequest/DetailPurchaseRequest/{Id}";
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
        public async Task<IActionResult> DetailPurchaseRequest(Guid Id)
        {
            ViewBag.Active = "PurchaseRequest";

            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);            
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);

            var purchaseRequest = await _purchaseRequestRepository.GetPurchaseRequestById(Id);

            if (purchaseRequest == null)
            {
                Response.StatusCode = 404;
                return View("PurchaseRequestNotFound", Id);
            }

            PurchaseRequest model = new PurchaseRequest
            {
                PurchaseRequestId = purchaseRequest.PurchaseRequestId,
                PurchaseRequestNumber = purchaseRequest.PurchaseRequestNumber,
                UserAccessId = purchaseRequest.UserAccessId,
                Department1Id = purchaseRequest.Department1Id,
                Position1Id = purchaseRequest.Position1Id,
                UserApprove1Id = purchaseRequest.UserApprove1Id,
                ApproveStatusUser1 = purchaseRequest.ApproveStatusUser1,
                Department2Id = purchaseRequest.Department2Id,
                Position2Id = purchaseRequest.Position2Id,
                UserApprove2Id = purchaseRequest.UserApprove2Id,
                ApproveStatusUser2 = purchaseRequest.ApproveStatusUser2,
                Department3Id = purchaseRequest.Department3Id,
                Position3Id = purchaseRequest.Position3Id,
                UserApprove3Id = purchaseRequest.UserApprove3Id,
                ApproveStatusUser3 = purchaseRequest.ApproveStatusUser3,
                ExpiredDay = purchaseRequest.ExpiredDay,
                RemainingDay = purchaseRequest.RemainingDay,
                ExpiredDate = purchaseRequest.ExpiredDate,
                TermOfPaymentId = purchaseRequest.TermOfPaymentId,
                Status = purchaseRequest.Status,
                QtyTotal = purchaseRequest.QtyTotal,
                GrandTotal = Math.Truncate(purchaseRequest.GrandTotal),
                Note = purchaseRequest.Note,
                MessageApprove1 = purchaseRequest.MessageApprove1,
                MessageApprove2 = purchaseRequest.MessageApprove2,
                MessageApprove3 = purchaseRequest.MessageApprove3             
            };

            var ItemsList = new List<PurchaseRequestDetail>();

            foreach (var item in purchaseRequest.PurchaseRequestDetails)
            {
                ItemsList.Add(new PurchaseRequestDetail
                {
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Measurement = item.Measurement,
                    Supplier = item.Supplier,                    
                    Qty = item.Qty,
                    Price = Math.Truncate(item.Price),
                    Discount = item.Discount,
                    SubTotal = Math.Truncate(item.SubTotal)
                });
            }

            model.PurchaseRequestDetails = ItemsList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DetailPurchaseRequest(PurchaseRequest model)
        {
            ViewBag.Active = "PurchaseRequest";

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var purchaseRequest = _purchaseRequestRepository.GetAllPurchaseRequest().Where(p => p.PurchaseRequestNumber == model.PurchaseRequestNumber).FirstOrDefault();
                var approval = _approvalRepository.GetAllApproval().Where(p => p.PurchaseRequestNumber == model.PurchaseRequestNumber).ToList();               

                if (purchaseRequest != null)
                {
                    if (approval != null)
                    {
                        foreach (var item in approval)
                        {
                            var itemApproval = approval.Where(o => o.PurchaseRequestId == item.PurchaseRequestId).FirstOrDefault();
                            if (itemApproval != null)
                            {
                                item.Note = model.Note;

                                _applicationDbContext.Entry(itemApproval).State = EntityState.Modified;
                                _applicationDbContext.SaveChanges();
                            }
                            else
                            {
                                ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                                ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                                ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);                                
                                ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                                ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                                TempData["WarningMessage"] = "Name " + item.ApproveBy + " Not Found !!!";
                                return View(model);
                            }
                        }

                        purchaseRequest.UpdateDateTime = DateTimeOffset.Now;
                        purchaseRequest.UpdateBy = new Guid(getUser.Id);                        
                        purchaseRequest.UserApprove1Id = model.UserApprove1Id;
                        purchaseRequest.UserApprove2Id = model.UserApprove2Id;
                        purchaseRequest.UserApprove3Id = model.UserApprove3Id;
                        purchaseRequest.TermOfPaymentId = model.TermOfPaymentId;                                              
                        purchaseRequest.QtyTotal = model.QtyTotal;
                        purchaseRequest.GrandTotal = model.GrandTotal;
                        purchaseRequest.ExpiredDate = model.ExpiredDate;
                        purchaseRequest.MessageApprove1 = model.MessageApprove1;
                        purchaseRequest.MessageApprove2 = model.MessageApprove2;
                        purchaseRequest.MessageApprove3 = model.MessageApprove3;
                        purchaseRequest.Note = model.Note;
                        purchaseRequest.PurchaseRequestDetails = model.PurchaseRequestDetails;                        

                        _purchaseRequestRepository.Update(purchaseRequest);

                        TempData["SuccessMessage"] = "Number " + model.PurchaseRequestNumber + " Changes saved";
                        return Json(new { redirectToUrl = Url.Action("Index", "PurchaseRequest") });
                    }
                    else
                    {
                        ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                        ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                        ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);                        
                        ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                        ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Number " + model.PurchaseRequestNumber + " Not Found !!!";
                        return View(model);
                    }
                }
                else
                {
                    ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                    ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                    ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);                    
                    ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                    ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Number " + model.PurchaseRequestNumber + " Already exists !!!";
                    return View(model);
                }
            }
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);            
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
            TempData["WarningMessage"] = "Number " + model.PurchaseRequestNumber + " Failed saved";
            return Json(new { redirectToUrl = Url.Action("Index", "PurchaseRequest") });
        }
        
        public async Task<IActionResult> PrintPurchaseRequest(Guid Id)
        {
            var purchaseRequest = await _purchaseRequestRepository.GetPurchaseRequestById(Id);

            var CreateDate = purchaseRequest.CreateDateTime.ToString("dd MMMM yyyy");
            var PrNumber = purchaseRequest.PurchaseRequestNumber;
            var CreateBy = purchaseRequest.ApplicationUser.NamaUser;
            var UserApprove1 = purchaseRequest.UserApprove1.FullName;
            var UserApprove2 = purchaseRequest.UserApprove2.FullName;
            var UserApprove3 = purchaseRequest.UserApprove3.FullName;
            var TermOfPayment = purchaseRequest.TermOfPayment.TermOfPaymentName;
            var Note = purchaseRequest.Note;
            var GrandTotal = purchaseRequest.GrandTotal;
            var Tax = (GrandTotal / 100) * 11;
            var GrandTotalAfterTax = (GrandTotal + Tax);

            WebReport web = new WebReport();
            var path = $"{_webHostEnvironment.WebRootPath}\\Reporting\\PurchaseRequest.frx";
            web.Report.Load(path);

            var msSqlDataConnection = new MsSqlDataConnection();
            msSqlDataConnection.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            var Conn = msSqlDataConnection.ConnectionString;

            web.Report.SetParameterValue("Conn", Conn);
            web.Report.SetParameterValue("PurchaseRequestId", Id.ToString());
            web.Report.SetParameterValue("PrNumber", PrNumber);
            web.Report.SetParameterValue("CreateDate", CreateDate);
            web.Report.SetParameterValue("CreateBy", CreateBy);
            web.Report.SetParameterValue("UserApprove1", UserApprove1);
            web.Report.SetParameterValue("UserApprove2", UserApprove2);
            web.Report.SetParameterValue("UserApprove3", UserApprove3);
            web.Report.SetParameterValue("TermOfPayment", TermOfPayment);
            web.Report.SetParameterValue("Note", Note);
            web.Report.SetParameterValue("GrandTotal", GrandTotal);
            web.Report.SetParameterValue("Tax", Tax);
            web.Report.SetParameterValue("GrandTotalAfterTax", GrandTotalAfterTax);

            web.Report.Prepare();

            Stream stream = new MemoryStream();
            web.Report.Export(new PDFSimpleExport(), stream);
            stream.Position = 0;
            return File(stream, "application/zip", (PrNumber + ".pdf"));
        }
    }
}
