﻿using Microsoft.AspNetCore.Authorization;
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
    public class UnitOrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUnitOrderRepository _unitOrderRepository;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IUnitLocationRepository _unitLocationRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly IUnitRequestRepository _unitRequestRepository;
        private readonly IWarehouseTransferRepository _warehouseTransferRepository;


        public UnitOrderController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUnitOrderRepository UnitOrderRepository,
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
            _unitOrderRepository = UnitOrderRepository;
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
            ViewBag.Active = "UnitOrder";
            var data = _unitOrderRepository.GetAllUnitOrder();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "UnitOrder";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _unitOrderRepository.GetAllUnitOrder().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> DetailUnitOrder(Guid Id)
        {
            ViewBag.Active = "UnitOrder";

            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);
            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);

            //var UnitOrder = await _unitOrderRepository.GetUnitOrderById(Id);

            UnitOrder UnitOrder = _applicationDbContext.UnitOrders
                .Include(d => d.UnitOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.UserApprove1)
                .Where(p => p.UnitOrderId == Id).FirstOrDefault();

            if (UnitOrder == null)
            {
                Response.StatusCode = 404;
                return View("UnitOrderNotFound", Id);
            }

            UnitOrder model = new UnitOrder
            {
                UnitOrderId = UnitOrder.UnitOrderId,
                UnitOrderNumber = UnitOrder.UnitOrderNumber,
                UnitRequestId = UnitOrder.UnitRequestId,
                UnitRequestNumber = UnitOrder.UnitRequestNumber,
                UserAccessId = UnitOrder.UserAccessId,
                UnitLocationId = UnitOrder.UnitLocationId,
                WarehouseLocationId = UnitOrder.WarehouseLocationId,
                UserApprove1Id = UnitOrder.UserApprove1Id,
                ApproveStatusUser1 = UnitOrder.ApproveStatusUser1,
                QtyTotal = UnitOrder.QtyTotal,
                Status = UnitOrder.Status,
                Note = UnitOrder.Note,
            };

            var ItemsList = new List<UnitOrderDetail>();

            foreach (var item in UnitOrder.UnitOrderDetails)
            {
                ItemsList.Add(new UnitOrderDetail
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

            model.UnitOrderDetails = ItemsList;
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

            UnitOrder UnitOrder = _applicationDbContext.UnitOrders
                .Include(d => d.UnitOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.UserApprove1)
                .Where(p => p.UnitOrderId == Id).FirstOrDefault();

            _signInManager.IsSignedIn(User);

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            WarehouseTransfer wtf = new WarehouseTransfer();

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _warehouseTransferRepository.GetAllWarehouseTransfer().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.WarehouseTransferNumber).FirstOrDefault();
            if (lastCode == null)
            {
                wtf.WarehouseTransferNumber = "TR" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.WarehouseTransferNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    wtf.WarehouseTransferNumber = "TR" + setDateNow + "0001";
                }
                else
                {
                    wtf.WarehouseTransferNumber = "TR" + setDateNow + (Convert.ToInt32(lastCode.WarehouseTransferNumber.Substring(9, lastCode.WarehouseTransferNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            ViewBag.WarehouseTransferNumber = wtf.WarehouseTransferNumber;

            var getWRQ = new UnitOrderViewModel()
            {
                UnitOrderId = UnitOrder.UnitOrderId,
                UnitOrderNumber = UnitOrder.UnitOrderNumber,
                UnitRequestId = UnitOrder.UnitRequestId,
                UnitRequestNumber = UnitOrder.UnitRequestNumber,
                UserAccessId = UnitOrder.UserAccessId,
                UnitLocationId = UnitOrder.UnitLocationId,                
                WarehouseLocationId = UnitOrder.WarehouseLocationId,
                UserApprove1Id = UnitOrder.UserApprove1Id,
                ApproveStatusUser1 = UnitOrder.ApproveStatusUser1,
                Status = UnitOrder.Status,
                QtyTotal = UnitOrder.QtyTotal,
                Note = UnitOrder.Note,
            };

            var ItemsList = new List<UnitOrderDetail>();

            foreach (var item in UnitOrder.UnitOrderDetails)
            {
                ItemsList.Add(new UnitOrderDetail
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

            getWRQ.UnitOrderDetails = ItemsList;

            return View(getWRQ);
        }

        [HttpPost]
        public async Task<IActionResult> TransferUnit(UnitOrder model, UnitOrderViewModel vm)
        {
            UnitOrder UnitOrder = await _unitOrderRepository.GetUnitOrderByIdNoTracking(model.UnitOrderId);

            _signInManager.IsSignedIn(User);

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            string getWarehouseTransferNumber = Request.Form["TRNumber"];

            var updateURequest = _unitRequestRepository.GetAllUnitRequest().Where(c => c.UnitRequestId == model.UnitRequestId).FirstOrDefault();
            if (updateURequest != null)
            {
                {
                    updateURequest.Status = UnitOrder.UnitOrderNumber;
                };
                _applicationDbContext.Entry(updateURequest).State = EntityState.Modified;
            }            

            var newWarehouseTransfer = new WarehouseTransfer
            {
                CreateDateTime = DateTime.Now,
                CreateBy = new Guid(getUser.Id),
                UnitOrderId = UnitOrder.UnitOrderId,
                UnitOrderNumber = UnitOrder.UnitOrderNumber,
                UserAccessId = getUser.Id.ToString(),
                UnitLocationId = UnitOrder.UnitLocationId,               
                WarehouseLocationId = UnitOrder.WarehouseLocationId,
                UserApprove1Id = UnitOrder.UserApprove1Id,
                Status = UnitOrder.UnitRequestNumber,
                QtyTotal = UnitOrder.QtyTotal,
            };

            newWarehouseTransfer.WarehouseTransferNumber = getWarehouseTransferNumber;

            var updateStatusUO = _unitOrderRepository.GetAllUnitOrder().Where(c => c.UnitOrderId == model.UnitOrderId).FirstOrDefault();
            if (updateStatusUO != null)
            {
                {
                    updateStatusUO.Status = newWarehouseTransfer.WarehouseTransferNumber;
                };
                _applicationDbContext.Entry(updateStatusUO).State = EntityState.Modified;
            }

            var ItemsList = new List<WarehouseTransferDetail>();

            foreach (var item in vm.UnitOrderDetails)
            {
                //Saat proses transfer, stok barang di gudang akan berkurang
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
            return RedirectToAction("Index", "UnitOrder");
        }
    }
}