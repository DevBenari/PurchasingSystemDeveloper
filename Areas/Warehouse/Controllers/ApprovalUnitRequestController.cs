using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
using PurchasingSystemDeveloper.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Route("Warehouse/[Controller]/[Action]")]
    public class ApprovalUnitRequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IApprovalUnitRequestRepository _approvalUnitRequestRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IUnitRequestRepository _unitRequestRepository;
        private readonly IUnitLocationRepository _unitLocationRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly IUnitOrderRepository _unitOrderRepository;

        private readonly IHostingEnvironment _hostingEnvironment;

        public ApprovalUnitRequestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IApprovalUnitRequestRepository ApprovalRequestRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            IPurchaseRequestRepository purchaseRequestRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IUnitRequestRepository unitRequestRepository,
            IUnitLocationRepository unitLocationRepository,
            IWarehouseLocationRepository warehouseLocationRepository,
            IUnitOrderRepository unitOrderRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _approvalUnitRequestRepository = ApprovalRequestRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _unitRequestRepository = unitRequestRepository;
            _unitLocationRepository = unitLocationRepository;
            _warehouseLocationRepository = warehouseLocationRepository;
            _unitOrderRepository = unitOrderRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "ApprovalUnitRequest";

            var getUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getUserLogin.KodeUser).FirstOrDefault();
            var getUser1 = _approvalUnitRequestRepository.GetAllApprovalRequest().Where(a => a.ApprovalStatusUser == "User1" && a.UserApproveId == getUserActive.UserActiveId && a.Status == "Waiting Approval").ToList();
            var getUser1Approve = _approvalUnitRequestRepository.GetAllApprovalRequest().Where(a => a.ApprovalStatusUser == "User1" && a.UserApproveId == getUserActive.UserActiveId && a.Status == "Approve").ToList();

            var itemList = new List<ApprovalUnitRequest>();

            if (getUser1 != null)
            {
                itemList.AddRange(getUser1);
                itemList.AddRange(getUser1Approve);
                
                return View(itemList);
            }            
            else
            {
                if (getUserLogin.Id == "5f734880-f3d9-4736-8421-65a66d48020e")
                {
                    var data = _approvalUnitRequestRepository.GetAllApprovalRequest();                    

                    return View(data);
                }
                else
                {
                    var data = _approvalUnitRequestRepository.GetAllApprovalRequest().Where(u => u.UserApproveId == getUserActive.UserActiveId && u.Status != "Waiting Approval" && u.Status != "Reject").ToList();                    

                    return View(data);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "ApprovalUnitRequest";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _approvalUnitRequestRepository.GetAllApprovalRequest().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<ViewResult> DetailApprovalUnitRequest(Guid Id)
        {
            ViewBag.Active = "ApprovalUnitRequest";

            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);

            var ApprovalRequest = await _approvalUnitRequestRepository.GetApprovalRequestById(Id);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ApprovalRequest == null)
            {
                Response.StatusCode = 404;
                return View("ApprovalRequestNotFound", Id);
            }

            ApprovalUnitRequestViewModel viewModel = new ApprovalUnitRequestViewModel()
            {
                ApprovalUnitRequestId = ApprovalRequest.ApprovalUnitRequestId,
                UnitRequestId = ApprovalRequest.UnitRequestId,
                UnitRequestNumber = ApprovalRequest.UnitRequestNumber,
                UserAccessId = ApprovalRequest.UserAccessId,
                UnitLocationId = ApprovalRequest.UnitLocationId,
                WarehouseLocationId = ApprovalRequest.WarehouseLocationId,
                UserApproveId = ApprovalRequest.UserApproveId,
                UserApprove = getUser.Email,
                ApprovalTime = "",
                ApproveBy = getUser.NamaUser,
                ApprovalDate = DateTime.Now,
                ApprovalStatusUser = ApprovalRequest.ApprovalStatusUser,
                Status = ApprovalRequest.Status,
                Note = ApprovalRequest.Note,
                Message = ApprovalRequest.Message,                
            };

            var getUrNumber = _unitRequestRepository.GetAllUnitRequest().Where(ur => ur.UnitRequestNumber == viewModel.UnitRequestNumber).FirstOrDefault();

            viewModel.QtyTotal = getUrNumber.QtyTotal;

            var ItemsList = new List<UnitRequestDetail>();

            foreach (var item in getUrNumber.UnitRequestDetails)
            {
                ItemsList.Add(new UnitRequestDetail
                {
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Supplier = item.Supplier,
                    Measurement = item.Measurement,
                    Qty = item.Qty,
                });
            }

            viewModel.UnitRequestDetails = ItemsList;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailApprovalUnitRequest(ApprovalUnitRequestViewModel viewModel)
        {
            ViewBag.Active = "ApprovalUnitRequest";

            if (ModelState.IsValid)
            {
                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                var approvalUnitReq = await _approvalUnitRequestRepository.GetApprovalRequestByIdNoTracking(viewModel.ApprovalUnitRequestId);
                var checkUnitReq = _unitRequestRepository.GetAllUnitRequest().Where(c => c.UnitRequestId == viewModel.UnitRequestId).FirstOrDefault();
                var diffDate = DateTimeOffset.Now.Date - approvalUnitReq.CreateDateTime.Date;

                if (checkUnitReq != null)
                {
                    if (approvalUnitReq.ApprovalStatusUser == "User1" && checkUnitReq.UnitRequestId == approvalUnitReq.UnitRequestId)
                    {
                        var updateStatusUser1 = _approvalUnitRequestRepository.GetAllApprovalRequest().Where(c => c.ApprovalStatusUser == "User1" && c.UnitRequestId == viewModel.UnitRequestId).FirstOrDefault();
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

                        checkUnitReq.ApproveStatusUser1 = viewModel.Status;
                        checkUnitReq.MessageApprove1 = viewModel.Message;

                        _applicationDbContext.Entry(checkUnitReq).State = EntityState.Modified;
                        _applicationDbContext.SaveChanges();
                    }                   

                    //Jika semua sudah Approve langsung Generate Unit Request
                    if (checkUnitReq.ApproveStatusUser1 == "Approve")
                    {
                        checkUnitReq.Status = viewModel.Status;

                        _applicationDbContext.Entry(checkUnitReq).State = EntityState.Modified;
                        _applicationDbContext.SaveChanges();
                        
                        //var openQtyDiff = _qtyDifferenceRepository.GetAllQtyDifference().FirstOrDefault();
                        //var getUO = _unio.GetAllPurchaseOrder().Where(p => p.PurchaseOrderId == viewModel.PurchaseOrderId).FirstOrDefault();
                        var getUR = _unitRequestRepository.GetAllUnitRequest().Where(a => a.UnitRequestId == viewModel.UnitRequestId).FirstOrDefault();

                        //Proses Generate Unit Order
                        var unitOrder = new UnitOrder
                        {
                            CreateDateTime = DateTimeOffset.Now,
                            CreateBy = getUR.CreateBy,
                            UnitRequestId = getUR.UnitRequestId,
                            UnitRequestNumber = getUR.UnitRequestNumber,
                            UserAccessId = getUR.CreateBy.ToString(),
                            UnitLocationId = getUR.UnitLocationId,
                            WarehouseLocationId = getUR.WarehouseLocationId,
                            UserApprove1Id = getUR.UserApprove1Id,
                            ApproveStatusUser1 = getUR.ApproveStatusUser1,
                            Status = "In Order",
                            QtyTotal = getUR.QtyTotal,
                            Note = getUR.Note
                        };

                        var ItemsList = new List<UnitOrderDetail>();

                        foreach (var item in getUR.UnitRequestDetails)
                        {
                            ItemsList.Add(new UnitOrderDetail
                            {
                                CreateDateTime = DateTimeOffset.Now,
                                CreateBy = new Guid(getUser.Id),
                                ProductNumber = item.ProductNumber,
                                ProductName = item.ProductName,
                                Supplier = item.Supplier,
                                Measurement = item.Measurement,
                                Qty = item.Qty,
                                //QtySent = item.QtySent,
                                Checked = item.Checked,
                            });
                        }

                        unitOrder.UnitOrderDetails = ItemsList;

                        var dateNow = DateTimeOffset.Now;
                        var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

                        var lastCode = _unitOrderRepository.GetAllUnitOrder().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.UnitOrderNumber).FirstOrDefault();
                        if (lastCode == null)
                        {
                            unitOrder.UnitOrderNumber = "UO" + setDateNow + "0001";
                        }
                        else
                        {
                            var lastCodeTrim = lastCode.UnitOrderNumber.Substring(2, 6);

                            if (lastCodeTrim != setDateNow)
                            {
                                unitOrder.UnitOrderNumber = "UO" + setDateNow + "0001";
                            }
                            else
                            {
                                unitOrder.UnitOrderNumber = "UO" + setDateNow + (Convert.ToInt32(lastCode.UnitOrderNumber.Substring(9, lastCode.UnitOrderNumber.Length - 9)) + 1).ToString("D4");
                            }
                        }

                        //Update Status UR Menjadi Nomor UO
                        var updateUR = _unitRequestRepository.GetAllUnitRequest().Where(c => c.UnitRequestId == viewModel.UnitRequestId).FirstOrDefault();
                        if (updateUR != null)
                        {
                            {
                                updateUR.Status = unitOrder.UnitOrderNumber;
                            };
                            _applicationDbContext.Entry(updateUR).State = EntityState.Modified;
                            _applicationDbContext.SaveChanges();
                        }

                        _unitOrderRepository.Tambah(unitOrder);

                        _approvalUnitRequestRepository.Update(approvalUnitReq);
                        _applicationDbContext.SaveChanges();

                        TempData["SuccessMessage"] = "Approve And Success Create Unit Order";
                        return RedirectToAction("Index", "ApprovalUnitRequest");
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Update Success";
                        return RedirectToAction("Index", "ApprovalUnitRequest");
                    }
                }
            }

            return View();
        }
    }
}
