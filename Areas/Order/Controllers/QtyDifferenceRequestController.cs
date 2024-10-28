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

namespace PurchasingSystemDeveloper.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("Order/[Controller]/[Action]")]
    public class QtyDifferenceRequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IQtyDifferenceRequestRepository _qtyDifferenceRequestRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IUnitRequestRepository _unitRequestRepository;
        private readonly IUnitLocationRepository _unitLocationRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly IQtyDifferenceRepository _qtyDifferenceRepository;

        private readonly IHostingEnvironment _hostingEnvironment;

        public QtyDifferenceRequestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IQtyDifferenceRequestRepository QtyDifferenceRequestRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IUnitRequestRepository unitRequestRepository,
            IUnitLocationRepository unitLocationRepository,
            IWarehouseLocationRepository warehouseLocationRepository,
            IQtyDifferenceRepository qtyDifferenceRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _qtyDifferenceRequestRepository = QtyDifferenceRequestRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _unitRequestRepository = unitRequestRepository;
            _unitLocationRepository = unitLocationRepository;
            _warehouseLocationRepository = warehouseLocationRepository;
            _qtyDifferenceRepository = qtyDifferenceRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "Order";
            var getUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getUserLogin.KodeUser).FirstOrDefault();

            if (getUserLogin.Id == "5f734880-f3d9-4736-8421-65a66d48020e")
            {
                var data = _qtyDifferenceRequestRepository.GetAllQtyDifferenceRequest();
                return View(data);
            }
            else
            {
                var data = _qtyDifferenceRequestRepository.GetAllQtyDifferenceRequest().Where(u => u.HeadWarehouseManagerId == getUserActive.UserActiveId).ToList();
                return View(data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "Order";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _qtyDifferenceRequestRepository.GetAllQtyDifferenceRequest().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<ViewResult> DetailQtyDifferenceRequest(Guid Id)
        {
            ViewBag.Active = "Order";

            ViewBag.QtyDifference = new SelectList(await _qtyDifferenceRepository.GetQtyDifferences(), "QtyDifferenceId", "QtyDifferenceNumber", SortOrder.Ascending);
            ViewBag.PO = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
            ViewBag.Head = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);           
            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);            

            var QtyDifferenceRequest = await _qtyDifferenceRequestRepository.GetQtyDifferenceRequestById(Id);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (QtyDifferenceRequest == null)
            {
                Response.StatusCode = 404;
                return View("QtyDifferenceRequestNotFound", Id);
            }

            QtyDifferenceRequestViewModel viewModel = new QtyDifferenceRequestViewModel()
            {
                QtyDifferenceRequestId = QtyDifferenceRequest.QtyDifferenceRequestId,
                QtyDifferenceId = QtyDifferenceRequest.QtyDifferenceId,
                PurchaseOrderId = QtyDifferenceRequest.PurchaseOrderId,
                UserAccessId = getUser.NamaUser,
                QtyDifferenceApproveDate = QtyDifferenceRequest.QtyDifferenceApproveDate,
                QtyDifferenceApproveBy = getUser.NamaUser,
                HeadWarehouseManagerId = QtyDifferenceRequest.HeadWarehouseManagerId,
                HeadPurchasingManagerId = QtyDifferenceRequest.HeadPurchasingManagerId,                
                Status = QtyDifferenceRequest.Status,
                Note = QtyDifferenceRequest.Note
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
        public async Task<IActionResult> DetailQtyDifferenceRequest(QtyDifferenceRequestViewModel viewModel)
        {
            ViewBag.Active = "Order";

            if (ModelState.IsValid)
            {
                QtyDifferenceRequest QtyDifferenceRequest = await _qtyDifferenceRequestRepository.GetQtyDifferenceRequestByIdNoTracking(viewModel.QtyDifferenceRequestId);

                QtyDifferenceRequest.Status = viewModel.Status;
                QtyDifferenceRequest.Note = viewModel.Note;

                if (QtyDifferenceRequest.Status == "Approved")
                {
                    QtyDifferenceRequest.QtyDifferenceApproveDate = DateTime.Now;
                    QtyDifferenceRequest.QtyDifferenceApproveBy = viewModel.QtyDifferenceApproveBy;

                    var getPO = _purchaseOrderRepository.GetAllPurchaseOrder().Where(p => p.PurchaseOrderId == viewModel.PurchaseOrderId).FirstOrDefault();
                    if (getPO != null)
                    {
                        getPO.Status = "Cancellation";

                        _applicationDbContext.Entry(getPO).State = EntityState.Modified;
                    }

                    var result = _qtyDifferenceRepository.GetAllQtyDifference().Where(c => c.QtyDifferenceId == viewModel.QtyDifferenceId).FirstOrDefault();
                    if (result != null)
                    {
                        result.Status = viewModel.Status;
                        result.Note = viewModel.Note;

                        _applicationDbContext.Entry(result).State = EntityState.Modified;
                    }

                    _qtyDifferenceRequestRepository.Update(QtyDifferenceRequest);
                    _applicationDbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Saved";
                }
                else if (QtyDifferenceRequest.Status == "Waiting Approval")
                {
                    TempData["SuccessMessage"] = "Saved";
                }
                else if (QtyDifferenceRequest.Status == "Not Approved") 
                {
                    QtyDifferenceRequest.QtyDifferenceApproveDate = DateTime.Now;
                    QtyDifferenceRequest.QtyDifferenceApproveBy = viewModel.QtyDifferenceApproveBy;

                    _qtyDifferenceRequestRepository.Update(QtyDifferenceRequest);
                    _applicationDbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Saved";
                }

                return RedirectToAction("Index", "QtyDifferenceRequest");
            }

            return View();
        }
    }
}
