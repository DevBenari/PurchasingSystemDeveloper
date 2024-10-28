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
    public class WarehouseRequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWarehouseRequestRepository _warehouseRequestRepository;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IUnitLocationRepository _unitLocationRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly IUnitRequestRepository _unitRequestRepository;
        private readonly IWarehouseTransferRepository _warehouseTransferRepository;


        public WarehouseRequestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IWarehouseRequestRepository WarehouseRequestRepository,
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
            _warehouseRequestRepository = WarehouseRequestRepository;
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
            var data = _warehouseRequestRepository.GetAllWarehouseRequest();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "Warehouse";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _warehouseRequestRepository.GetAllWarehouseRequest().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> DetailWarehouseRequest(Guid Id)
        {
            ViewBag.Active = "Warehouse";

            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);
            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);

            //var WarehouseRequest = await _warehouseRequestRepository.GetWarehouseRequestById(Id);

            WarehouseRequest WarehouseRequest = _applicationDbContext.WarehouseRequests
                .Include(d => d.WarehouseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(b => b.UnitRequestManager)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.WarehouseApproval)
                .Where(p => p.WarehouseRequestId == Id).FirstOrDefault();

            if (WarehouseRequest == null)
            {
                Response.StatusCode = 404;
                return View("WarehouseRequestNotFound", Id);
            }

            WarehouseRequest model = new WarehouseRequest
            {
                WarehouseRequestId = WarehouseRequest.WarehouseRequestId,
                WarehouseRequestNumber = WarehouseRequest.WarehouseRequestNumber,
                UnitRequestId = WarehouseRequest.UnitRequestId,
                UnitRequestNumber = WarehouseRequest.UnitRequestNumber,
                UserAccessId = WarehouseRequest.UserAccessId,
                UnitLocationId = WarehouseRequest.UnitLocationId,
                UnitRequestManagerId = WarehouseRequest.UnitRequestManagerId,
                WarehouseLocationId = WarehouseRequest.WarehouseLocationId,
                WarehouseApprovalId = WarehouseRequest.WarehouseApprovalId,
                QtyTotal = WarehouseRequest.QtyTotal,
                Status = WarehouseRequest.Status
            };

            var ItemsList = new List<WarehouseRequestDetail>();

            foreach (var item in WarehouseRequest.WarehouseRequestDetails)
            {
                ItemsList.Add(new WarehouseRequestDetail
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

            model.WarehouseRequestDetails = ItemsList;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TransferUnit(Guid Id)
        {
            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);
            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);

            WarehouseRequest WarehouseRequest = _applicationDbContext.WarehouseRequests
                .Include(d => d.WarehouseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(b => b.UnitRequestManager)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.WarehouseApproval)
                .Where(p => p.WarehouseRequestId == Id).FirstOrDefault();

            _signInManager.IsSignedIn(User);

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            WarehouseTransfer wtf = new WarehouseTransfer();

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _warehouseTransferRepository.GetAllWarehouseTransfer().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.WarehouseTransferNumber).FirstOrDefault();
            if (lastCode == null)
            {
                wtf.WarehouseTransferNumber = "WTF" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.WarehouseTransferNumber.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    wtf.WarehouseTransferNumber = "WTF" + setDateNow + "0001";
                }
                else
                {
                    wtf.WarehouseTransferNumber = "WTF" + setDateNow + (Convert.ToInt32(lastCode.WarehouseTransferNumber.Substring(9, lastCode.WarehouseTransferNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            ViewBag.WarehouseTransferNumber = wtf.WarehouseTransferNumber;

            var getWRQ = new WarehouseRequestViewModel()
            {
                WarehouseRequestId = WarehouseRequest.WarehouseRequestId,
                WarehouseRequestNumber = WarehouseRequest.WarehouseRequestNumber,
                UnitRequestId = WarehouseRequest.UnitRequestId,
                UnitRequestNumber = WarehouseRequest.UnitRequestNumber,
                UserAccessId = WarehouseRequest.UserAccessId,
                UnitLocationId = WarehouseRequest.UnitLocationId,
                UnitRequestManagerId = WarehouseRequest.UnitRequestManagerId,
                WarehouseLocationId = WarehouseRequest.WarehouseLocationId,
                WarehouseApprovalId = WarehouseRequest.WarehouseApprovalId,
                Status = WarehouseRequest.Status,
                QtyTotal = WarehouseRequest.QtyTotal,
            };

            var ItemsList = new List<WarehouseRequestDetail>();

            foreach (var item in WarehouseRequest.WarehouseRequestDetails)
            {
                ItemsList.Add(new WarehouseRequestDetail
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Supplier = item.Supplier,
                    Measurement = item.Measurement,
                    Qty = item.Qty
                });
            }

            getWRQ.WarehouseRequestDetails = ItemsList;

            return View(getWRQ);
        }

        [HttpPost]
        public async Task<IActionResult> TransferUnit(WarehouseRequest model, WarehouseRequestViewModel vm)
        {
            WarehouseRequest warehouseRequest = await _warehouseRequestRepository.GetWarehouseRequestByIdNoTracking(model.WarehouseRequestId);

            _signInManager.IsSignedIn(User);

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            string getWarehouseTransferNumber = Request.Form["WTFNumber"];

            var updateURequest = _unitRequestRepository.GetAllUnitRequest().Where(c => c.UnitRequestId == model.UnitRequestId).FirstOrDefault();
            if (updateURequest != null)
            {
                {
                    updateURequest.Status = warehouseRequest.WarehouseRequestNumber;
                };
                _applicationDbContext.Entry(updateURequest).State = EntityState.Modified;
            }            

            var newWarehouseTransfer = new WarehouseTransfer
            {
                CreateDateTime = DateTime.Now,
                CreateBy = new Guid(getUser.Id),
                WarehouseRequestId = warehouseRequest.WarehouseRequestId,
                WarehouseRequestNumber = warehouseRequest.WarehouseRequestNumber,
                UserAccessId = getUser.Id.ToString(),
                UnitLocationId = warehouseRequest.UnitLocationId,
                UnitRequestManagerId = warehouseRequest.UnitRequestManagerId,
                WarehouseLocationId = warehouseRequest.WarehouseLocationId,
                WarehouseApprovalId = warehouseRequest.WarehouseApprovalId,
                Status = warehouseRequest.UnitRequestNumber,
                QtyTotal = warehouseRequest.QtyTotal
            };

            newWarehouseTransfer.WarehouseTransferNumber = getWarehouseTransferNumber;

            var updateWRQ = _warehouseRequestRepository.GetAllWarehouseRequest().Where(c => c.WarehouseRequestId == model.WarehouseRequestId).FirstOrDefault();
            if (updateWRQ != null)
            {
                {
                    updateWRQ.Status = newWarehouseTransfer.WarehouseTransferNumber;
                };
                _applicationDbContext.Entry(updateWRQ).State = EntityState.Modified;
            }

            var ItemsList = new List<WarehouseTransferDetail>();

            foreach (var item in vm.WarehouseRequestDetails)
            {
                //Saat proses transfer stok barang di gudang akan berkurang
                var updateProduk = _productRepository.GetAllProduct().Where(c => c.ProductCode == item.ProductNumber).FirstOrDefault();
                if (updateProduk != null)
                {
                    {
                        updateProduk.Stock = updateProduk.Stock - item.Qty;
                    };
                    _applicationDbContext.Entry(updateProduk).State = EntityState.Modified;
                }

                ItemsList.Add(new WarehouseTransferDetail
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Supplier = item.Supplier,
                    Measurement = item.Measurement,
                    Qty = item.Qty,
                    QtySent = item.QtySent,
                    Checked = item.Checked
                });
            }

            newWarehouseTransfer.WarehouseTransferDetails = ItemsList;
            _warehouseTransferRepository.Tambah(newWarehouseTransfer);            

            TempData["SuccessMessage"] = "Number " + newWarehouseTransfer.WarehouseTransferNumber + " Saved";
            return RedirectToAction("Index", "WarehouseRequest");
        }
    }
}
