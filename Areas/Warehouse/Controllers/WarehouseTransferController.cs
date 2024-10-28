using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

namespace PurchasingSystemDeveloper.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Route("Warehouse/[Controller]/[Action]")]
    public class WarehouseTransferController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWarehouseTransferRepository _WarehouseTransferRepository;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IUnitLocationRepository _unitLocationRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly IUnitRequestRepository _unitRequestRepository;
        private readonly IWarehouseTransferRepository _warehouseTransferRepository;


        public WarehouseTransferController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IWarehouseTransferRepository WarehouseTransferRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IPurchaseRequestRepository purchaseRequestRepository,
            IUnitLocationRepository unitLocationRepository,
            IWarehouseLocationRepository warehouseLocationRepository,
            IUnitRequestRepository unitRequestRepository,
            IWarehouseTransferRepository warehouseTransferRepository
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _WarehouseTransferRepository = WarehouseTransferRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _unitLocationRepository = unitLocationRepository;
            _warehouseLocationRepository = warehouseLocationRepository;
            _unitRequestRepository = unitRequestRepository;
            _warehouseTransferRepository = warehouseTransferRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Active = "Warehouse";
            var data = _WarehouseTransferRepository.GetAllWarehouseTransfer();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "Warehouse";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _WarehouseTransferRepository.GetAllWarehouseTransfer().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> DetailWarehouseTransfer(Guid Id)
        {
            ViewBag.Active = "Warehouse";

            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);
            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);

            //var WarehouseTransfer = await _WarehouseTransferRepository.GetWarehouseTransferById(Id);

            WarehouseTransfer WarehouseTransfer = _applicationDbContext.WarehouseTransfers
                .Include(d => d.WarehouseTransferDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(b => b.UnitRequestManager)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.WarehouseApproval)
                .Where(p => p.WarehouseTransferId == Id).FirstOrDefault();

            if (WarehouseTransfer == null)
            {
                Response.StatusCode = 404;
                return View("WarehouseTransferNotFound", Id);
            }

            WarehouseTransfer model = new WarehouseTransfer
            {
                WarehouseTransferId = WarehouseTransfer.WarehouseTransferId,
                WarehouseTransferNumber = WarehouseTransfer.WarehouseTransferNumber,
                WarehouseRequestId = WarehouseTransfer.WarehouseRequestId,
                WarehouseRequestNumber = WarehouseTransfer.WarehouseRequestNumber,
                UserAccessId = WarehouseTransfer.UserAccessId,
                UnitLocationId = WarehouseTransfer.UnitLocationId,
                UnitRequestManagerId = WarehouseTransfer.UnitRequestManagerId,
                WarehouseLocationId = WarehouseTransfer.WarehouseLocationId,
                WarehouseApprovalId = WarehouseTransfer.WarehouseApprovalId,
                QtyTotal = WarehouseTransfer.QtyTotal,
                Status = WarehouseTransfer.Status
            };

            var ItemsList = new List<WarehouseTransferDetail>();

            foreach (var item in WarehouseTransfer.WarehouseTransferDetails)
            {
                ItemsList.Add(new WarehouseTransferDetail
                {
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Measurement = item.Measurement,
                    Supplier = item.Supplier,
                    Qty = item.Qty,
                    QtySent = item.QtySent,
                    Checked = item.Checked
                });
            }

            model.WarehouseTransferDetails = ItemsList;
            return View(model);
        }
    }
}
