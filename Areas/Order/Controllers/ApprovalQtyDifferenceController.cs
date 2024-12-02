using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Order.ViewModels;
using PurchasingSystemDeveloper.Areas.Transaction.Models;
using PurchasingSystemDeveloper.Areas.Transaction.Repositories;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Repositories;
using PurchasingSystemDeveloper.Areas.Warehouse.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Hubs;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Security.Cryptography;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("Order/[Controller]/[Action]")]
    public class ApprovalQtyDifferenceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IUnitRequestRepository _unitRequestRepository;
        private readonly IUnitLocationRepository _unitLocationRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly IQtyDifferenceRepository _qtyDifferenceRepository;
        private readonly IApprovalQtyDifferenceRepository _approvalQtyDifferenceRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ApprovalQtyDifferenceController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IPurchaseRequestRepository purchaseRequestRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IUnitRequestRepository unitRequestRepository,
            IUnitLocationRepository unitLocationRepository,
            IWarehouseLocationRepository warehouseLocationRepository,
            IQtyDifferenceRepository qtyDifferenceRepository,
            IApprovalQtyDifferenceRepository approvalQtyDifferenceRepository,
            IHubContext<ChatHub> hubContext,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _unitRequestRepository = unitRequestRepository;
            _unitLocationRepository = unitLocationRepository;
            _warehouseLocationRepository = warehouseLocationRepository;
            _qtyDifferenceRepository = qtyDifferenceRepository;
            _approvalQtyDifferenceRepository = approvalQtyDifferenceRepository;
            _hubContext = hubContext;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                ViewBag.Active = "ApprovalQtyDifference";
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:Order/ApprovalQtyDifference/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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
            ViewBag.Active = "ApprovalQtyDifference";
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

            if (getUserLogin.Email == "superadmin@admin.com")
            {
                var data = await _approvalQtyDifferenceRepository.GetAllApprovalQtyDifferencePageSize(searchTerm, page, pageSize, startDate, endDate);               

                var model = new Pagination<ApprovalQtyDifference>
                {
                    Items = data.approvalQtyDifferences,
                    TotalCount = data.totalCountApprovalQtyDifferences,
                    PageSize = pageSize,
                    CurrentPage = page,
                };

                return View(model);
            }
            else
            {
                var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getUserLogin.KodeUser).FirstOrDefault();
                var getUser1 = _approvalQtyDifferenceRepository.GetAllApproval().Where(a => a.ApprovalStatusUser == "User1" && a.UserApproveId == getUserActive.UserActiveId && a.Status == "Waiting Approval").ToList();
                var getUser2 = _approvalQtyDifferenceRepository.GetAllApproval().Where(a => a.ApprovalStatusUser == "User2" && a.UserApproveId == getUserActive.UserActiveId && a.Status == "User1Approve").ToList();

                var getUser1Approve = _approvalQtyDifferenceRepository.GetAllApproval().Where(a => a.ApprovalStatusUser == "User1" && a.UserApproveId == getUserActive.UserActiveId && a.Status == "Approve").ToList();
                var getUser2Approve = _approvalQtyDifferenceRepository.GetAllApproval().Where(a => a.ApprovalStatusUser == "User2" && a.UserApproveId == getUserActive.UserActiveId && a.Status == "Approve").ToList();

                var itemList = new List<ApprovalQtyDifference>();

                if (getUser1 != null && getUser2 != null)
                {
                    itemList.AddRange(getUser1);
                    itemList.AddRange(getUser2);

                    itemList.AddRange(getUser1Approve);
                    itemList.AddRange(getUser2Approve);                    

                    var model = new Pagination<ApprovalQtyDifference>
                    {
                        Items = itemList,
                        TotalCount = itemList.Count,
                        PageSize = pageSize,
                        CurrentPage = page,
                    };

                    return View(model);
                }
                else if (getUser1 != null && getUser2 != null)
                {
                    itemList.AddRange(getUser1);
                    itemList.AddRange(getUser2);

                    itemList.AddRange(getUser1Approve);
                    itemList.AddRange(getUser2Approve);                    

                    var model = new Pagination<ApprovalQtyDifference>
                    {
                        Items = itemList,
                        TotalCount = itemList.Count,
                        PageSize = pageSize,
                        CurrentPage = page,
                    };

                    return View(model);
                }
                else if (getUser1 != null)
                {
                    itemList.AddRange(getUser1);
                    itemList.AddRange(getUser1Approve);                    

                    var model = new Pagination<ApprovalQtyDifference>
                    {
                        Items = itemList,
                        TotalCount = itemList.Count,
                        PageSize = pageSize,
                        CurrentPage = page,
                    };

                    return View(model);
                }
            }
            return View();
        }

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                ViewBag.Active = "ApprovalQtyDifference";
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:Order/ApprovalQtyDifference/DetailApprovalQtyDifference/{Id}";
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
        public async Task<ViewResult> DetailApprovalQtyDifference(Guid Id)
        {
            ViewBag.Active = "ApprovalQtyDifference";

            ViewBag.QtyDifference = new SelectList(await _qtyDifferenceRepository.GetQtyDifferences(), "QtyDifferenceId", "QtyDifferenceNumber", SortOrder.Ascending);
            ViewBag.PO = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
            ViewBag.Head = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);

            var QtyDifferenceRequest = await _approvalQtyDifferenceRepository.GetApprovalById(Id);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (QtyDifferenceRequest == null)
            {
                Response.StatusCode = 404;
                return View("QtyDifferenceRequestNotFound", Id);
            }

            ApprovalQtyDifferenceViewModel viewModel = new ApprovalQtyDifferenceViewModel()
            {
                ApprovalQtyDifferenceId = QtyDifferenceRequest.ApprovalQtyDifferenceId,
                QtyDifferenceId = QtyDifferenceRequest.QtyDifferenceId,
                QtyDifferenceNumber = QtyDifferenceRequest.QtyDifferenceNumber,
                UserAccessId = QtyDifferenceRequest.UserAccessId,
                PurchaseOrderId = QtyDifferenceRequest.PurchaseOrderId,
                PurchaseOrderNumber = QtyDifferenceRequest.PurchaseOrderNumber,
                UserApproveId = QtyDifferenceRequest.UserApproveId,
                UserApprove = getUser.Email,
                ApprovalTime = "",
                ApproveBy = getUser.NamaUser,
                ApprovalDate = DateTime.Now,
                ApprovalStatusUser = QtyDifferenceRequest.ApprovalStatusUser,
                Status = QtyDifferenceRequest.Status,
                Note = QtyDifferenceRequest.Note,
                Message = QtyDifferenceRequest.Message,
            };

            var getQtyDiff = _qtyDifferenceRepository.GetAllQtyDifference().Where(ur => ur.PurchaseOrderId == viewModel.PurchaseOrderId).FirstOrDefault();

            //viewModel.QtyTotal = getUrNumber.QtyTotal;

            var ItemsList = new List<QtyDifferenceDetail>();

            foreach (var item in getQtyDiff.QtyDifferenceDetails)
            {
                ItemsList.Add(new QtyDifferenceDetail
                {
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Measurement = item.Measurement,
                    Supplier = item.Supplier,
                    QtyOrder = item.QtyOrder,
                    QtyReceive = item.QtyReceive,
                    Price = item.Price,
                    Discount = item.Discount,
                });
            }

            viewModel.QtyDifferenceDetails = ItemsList;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailApprovalQtyDifference(ApprovalQtyDifferenceViewModel viewModel)
        {
            ViewBag.Active = "ApprovalQtyDifference";

            if (ModelState.IsValid)
            {
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                var approvalQtyDiff = await _approvalQtyDifferenceRepository.GetApprovalByIdNoTracking(viewModel.ApprovalQtyDifferenceId);
                var checkQtyDiff = _qtyDifferenceRepository.GetAllQtyDifference().Where(c => c.QtyDifferenceId == viewModel.QtyDifferenceId).FirstOrDefault();
                var diffDate = DateTimeOffset.Now.Date - approvalQtyDiff.CreateDateTime.Date;

                if (checkQtyDiff != null)
                {
                    if (approvalQtyDiff.ApprovalStatusUser == "User1" && checkQtyDiff.QtyDifferenceId == approvalQtyDiff.QtyDifferenceId)
                    {
                        var updateStatusUser1 = _approvalQtyDifferenceRepository.GetAllApproval().Where(c => c.ApprovalStatusUser == "User1" && c.QtyDifferenceId == viewModel.QtyDifferenceId).FirstOrDefault();
                        if (updateStatusUser1.Status == "Waiting Approval")
                        {
                            updateStatusUser1.Status = viewModel.Status;
                            updateStatusUser1.ApprovalDate = DateTimeOffset.Now;
                            updateStatusUser1.ApproveBy = getUser.NamaUser;
                            updateStatusUser1.ApprovalTime = diffDate.Days.ToString() + " Day";
                            updateStatusUser1.Message = viewModel.Message;

                            _applicationDbContext.Entry(updateStatusUser1).State = EntityState.Modified;
                            _applicationDbContext.SaveChanges();
                        }

                        if (viewModel.Status == "Approve")
                        {
                            var updateStatusUser2 = _approvalQtyDifferenceRepository.GetAllApproval().Where(c => c.ApprovalStatusUser == "User2" && c.QtyDifferenceId == viewModel.QtyDifferenceId).FirstOrDefault();
                            if (updateStatusUser2 != null)
                            {
                                updateStatusUser2.Status = "User1Approve";

                                _applicationDbContext.Entry(updateStatusUser2).State = EntityState.Modified;
                                _applicationDbContext.SaveChanges();
                            }

                            checkQtyDiff.ApproveStatusUser1 = viewModel.Status;
                            checkQtyDiff.MessageApprove1 = viewModel.Message;

                            _applicationDbContext.Entry(checkQtyDiff).State = EntityState.Modified;
                            _applicationDbContext.SaveChanges();
                        }
                        else
                        {
                            checkQtyDiff.ApproveStatusUser1 = viewModel.Status;
                            checkQtyDiff.MessageApprove1 = viewModel.Message;

                            _applicationDbContext.Entry(checkQtyDiff).State = EntityState.Modified;
                            _applicationDbContext.SaveChanges();
                        }
                    }
                    else if (approvalQtyDiff.ApprovalStatusUser == "User2" && checkQtyDiff.QtyDifferenceId == approvalQtyDiff.QtyDifferenceId)
                    {
                        var updateStatusUser2 = _approvalQtyDifferenceRepository.GetAllApproval().Where(c => c.ApprovalStatusUser == "User2" && c.QtyDifferenceId == viewModel.QtyDifferenceId).FirstOrDefault();
                        if (updateStatusUser2.Status == "User1Approve")
                        {
                            updateStatusUser2.Status = viewModel.Status;
                            updateStatusUser2.ApprovalDate = DateTimeOffset.Now;
                            updateStatusUser2.ApproveBy = getUser.NamaUser;
                            updateStatusUser2.ApprovalTime = diffDate.Days.ToString() + " Day";
                            updateStatusUser2.Message = viewModel.Message;

                            _applicationDbContext.Entry(updateStatusUser2).State = EntityState.Modified;
                            _applicationDbContext.SaveChanges();
                        }

                        checkQtyDiff.ApproveStatusUser2 = viewModel.Status;
                        checkQtyDiff.MessageApprove2 = viewModel.Message;

                        _applicationDbContext.Entry(checkQtyDiff).State = EntityState.Modified;
                        _applicationDbContext.SaveChanges();
                    }

                    //Jika semua sudah Approve langsung Generate Purchase Order
                    if (checkQtyDiff.ApproveStatusUser1 == "Approve" && checkQtyDiff.ApproveStatusUser2 == "Approve")
                    {
                        checkQtyDiff.Status = viewModel.Status;

                        _applicationDbContext.Entry(checkQtyDiff).State = EntityState.Modified;
                        _applicationDbContext.SaveChanges();

                        var updateStatusPo = _purchaseOrderRepository.GetAllPurchaseOrder().Where(p => p.PurchaseOrderId == checkQtyDiff.PurchaseOrderId).FirstOrDefault();
                        if (updateStatusPo != null)
                        {
                            updateStatusPo.Status = "Cancelled";
                            _applicationDbContext.Entry(updateStatusPo).State = EntityState.Modified;
                            _applicationDbContext.SaveChanges();
                        }

                        //var openQtyDiff = _qtyDifferenceRepository.GetAllQtyDifference().FirstOrDefault();
                        var getPO = _purchaseOrderRepository.GetAllPurchaseOrder().Where(p => p.PurchaseOrderId == checkQtyDiff.PurchaseOrderId).FirstOrDefault();
                        var getPR = _purchaseRequestRepository.GetAllPurchaseRequest().Where(a => a.PurchaseRequestId == getPO.PurchaseRequestId).FirstOrDefault();
                        var getQtyDiff = _qtyDifferenceRepository.GetAllQtyDifference().Where(po => po.PurchaseOrderId == getPO.PurchaseOrderId).FirstOrDefault();

                        //Proses Generate New Purchase Order
                        var newPO = new PurchaseOrder
                        {
                            CreateDateTime = DateTimeOffset.Now,
                            CreateBy = getPR.CreateBy,
                            PurchaseRequestId = getPR.PurchaseRequestId,
                            PurchaseRequestNumber = getPR.PurchaseRequestNumber,
                            UserAccessId = getPR.CreateBy.ToString(),
                            ExpiredDate = getPR.ExpiredDate,
                            UserApprove1Id = getPR.UserApprove1Id,
                            UserApprove2Id = getPR.UserApprove2Id,
                            UserApprove3Id = getPR.UserApprove3Id,
                            ApproveStatusUser1 = getPR.ApproveStatusUser1,
                            ApproveStatusUser2 = getPR.ApproveStatusUser2,
                            ApproveStatusUser3 = getPR.ApproveStatusUser3,
                            TermOfPaymentId = getPR.TermOfPaymentId,
                            Status = "In Order",
                            QtyTotal = getPR.QtyTotal,
                            GrandTotal = Math.Truncate(getPR.GrandTotal),
                            Note = "Reference from PO Number " + getPO.PurchaseOrderNumber,
                        };

                        var ItemsList = new List<PurchaseOrderDetail>();

                        foreach (var item in getPO.PurchaseOrderDetails)
                        {
                            if (getPO.PurchaseOrderDetails.Count != 1)
                            {
                                foreach (var itemQtyDiff in getQtyDiff.QtyDifferenceDetails)
                                {
                                    if (itemQtyDiff.ProductName == item.ProductName && itemQtyDiff.QtyReceive == item.Qty)
                                    {
                                        ItemsList.Add(new PurchaseOrderDetail
                                        {
                                            CreateDateTime = DateTimeOffset.Now,
                                            CreateBy = new Guid(getUser.Id),
                                            ProductNumber = item.ProductNumber,
                                            ProductName = item.ProductName,
                                            Supplier = item.Supplier,
                                            Measurement = item.Measurement,
                                            Qty = item.Qty,
                                            Price = Math.Truncate(item.Price),
                                            Discount = item.Discount,
                                            SubTotal = Math.Truncate(item.SubTotal)
                                        });
                                    }
                                    else if (itemQtyDiff.ProductName == item.ProductName && itemQtyDiff.QtyReceive != item.Qty)
                                    {
                                        ItemsList.Add(new PurchaseOrderDetail
                                        {
                                            CreateDateTime = DateTimeOffset.Now,
                                            CreateBy = new Guid(getUser.Id),
                                            ProductNumber = item.ProductNumber,
                                            ProductName = item.ProductName,
                                            Supplier = item.Supplier,
                                            Measurement = item.Measurement,
                                            Qty = itemQtyDiff.QtyReceive,
                                            Price = Math.Truncate(item.Price),
                                            Discount = item.Discount,
                                            SubTotal = Math.Truncate(item.SubTotal)
                                        });
                                    }
                                }
                            }
                            else
                            {
                                ItemsList.Add(new PurchaseOrderDetail
                                {
                                    CreateDateTime = DateTimeOffset.Now,
                                    CreateBy = new Guid(getUser.Id),
                                    ProductNumber = item.ProductNumber,
                                    ProductName = item.ProductName,
                                    Supplier = item.Supplier,
                                    Measurement = item.Measurement,
                                    Qty = item.Qty,
                                    Price = Math.Truncate(item.Price),
                                    Discount = item.Discount,
                                    SubTotal = Math.Truncate(item.SubTotal)
                                });
                            }
                        }

                        newPO.PurchaseOrderDetails = ItemsList;

                        var dateNow = DateTimeOffset.Now;
                        var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

                        var lastCode = _purchaseOrderRepository.GetAllPurchaseOrder().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.PurchaseOrderNumber).FirstOrDefault();
                        if (lastCode == null)
                        {
                            newPO.PurchaseOrderNumber = "PO" + setDateNow + "0001";
                        }
                        else
                        {
                            var lastCodeTrim = lastCode.PurchaseOrderNumber.Substring(2, 6);

                            if (lastCodeTrim != setDateNow)
                            {
                                newPO.PurchaseOrderNumber = "PO" + setDateNow + "0001";
                            }
                            else
                            {
                                newPO.PurchaseOrderNumber = "PO" + setDateNow + (Convert.ToInt32(lastCode.PurchaseOrderNumber.Substring(9, lastCode.PurchaseOrderNumber.Length - 9)) + 1).ToString("D4");
                            }
                        }

                        _purchaseOrderRepository.Tambah(newPO);

                        _approvalQtyDifferenceRepository.Update(approvalQtyDiff);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Approve And Success Create New Purchase Order";
                        return RedirectToAction("Index", "ApprovalQtyDifference");
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Update Success";
                        return RedirectToAction("Index", "ApprovalQtyDifference");
                    }
                }                
            }

            return View();
        }
    }
}
