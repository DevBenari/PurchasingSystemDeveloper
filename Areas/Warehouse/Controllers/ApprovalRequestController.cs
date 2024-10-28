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
    public class ApprovalRequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IApprovalRequestRepository _ApprovalRequestRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IUnitRequestRepository _unitRequestRepository;
        private readonly IUnitLocationRepository _unitLocationRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;

        private readonly IHostingEnvironment _hostingEnvironment;

        public ApprovalRequestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IApprovalRequestRepository ApprovalRequestRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            IPurchaseRequestRepository purchaseRequestRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IUnitRequestRepository unitRequestRepository,
            IUnitLocationRepository unitLocationRepository,
            IWarehouseLocationRepository warehouseLocationRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _ApprovalRequestRepository = ApprovalRequestRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _unitRequestRepository = unitRequestRepository;
            _unitLocationRepository = unitLocationRepository;
            _warehouseLocationRepository = warehouseLocationRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "Warehouse";
            var getUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getUserLogin.KodeUser).FirstOrDefault();

            if (getUserLogin.Id == "5f734880-f3d9-4736-8421-65a66d48020e")
            {
                var data = _ApprovalRequestRepository.GetAllApprovalRequest();
                return View(data);
            }
            else
            {
                var data = _ApprovalRequestRepository.GetAllApprovalRequest().Where(u => u.WarehouseApprovalId == getUserActive.UserActiveId).ToList();
                return View(data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "Warehouse";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _ApprovalRequestRepository.GetAllApprovalRequest().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<ViewResult> DetailApprovalRequest(Guid Id)
        {
            ViewBag.Active = "Warehouse";

            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);

            var ApprovalRequest = await _ApprovalRequestRepository.GetApprovalRequestById(Id);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ApprovalRequest == null)
            {
                Response.StatusCode = 404;
                return View("ApprovalRequestNotFound", Id);
            }

            ApprovalRequestViewModel viewModel = new ApprovalRequestViewModel()
            {
                ApprovalRequestId = ApprovalRequest.ApprovalRequestId,
                UnitRequestId = ApprovalRequest.UnitRequestId,
                UnitRequestNumber = ApprovalRequest.UnitRequestNumber,
                UserAccessId = ApprovalRequest.UserAccessId,
                UnitLocationId = ApprovalRequest.UnitLocationId,
                UnitRequestManagerId = ApprovalRequest.UnitRequestManagerId,
                ApproveDate = ApprovalRequest.ApproveDate,
                WarehouseApprovalId = ApprovalRequest.WarehouseApprovalId,
                WarehouseApproveBy = getUser.NamaUser,
                Status = ApprovalRequest.Status,
                Note = ApprovalRequest.Note
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
        public async Task<IActionResult> DetailApprovalRequest(ApprovalRequestViewModel viewModel)
        {
            ViewBag.Active = "Warehouse";

            if (ModelState.IsValid)
            {
                ApprovalRequest ApprovalRequest = await _ApprovalRequestRepository.GetApprovalRequestByIdNoTracking(viewModel.ApprovalRequestId);

                ApprovalRequest.Status = viewModel.Status;
                ApprovalRequest.Note = viewModel.Note;

                if (ApprovalRequest.Status == "Approved" || ApprovalRequest.Status == "Rejected")
                {
                    ApprovalRequest.ApproveDate = DateTime.Now;
                    ApprovalRequest.WarehouseApproveBy = viewModel.WarehouseApproveBy;
                }

                var result = _unitRequestRepository.GetAllUnitRequest().Where(c => c.UnitRequestNumber == viewModel.UnitRequestNumber).FirstOrDefault();
                if (result != null)
                {
                    result.Status = viewModel.Status;
                    result.Note = viewModel.Note;

                    _applicationDbContext.Entry(result).State = EntityState.Modified;
                }
                _ApprovalRequestRepository.Update(ApprovalRequest);

                if (ApprovalRequest.Status == "Request")
                {
                    TempData["SuccessMessage"] = "Number " + viewModel.UnitRequestNumber + " Request";
                }
                else if (ApprovalRequest.Status == "Approved")
                {
                    TempData["SuccessMessage"] = "Number " + viewModel.UnitRequestNumber + " Approved";
                }
                else if (ApprovalRequest.Status == "Rejected")
                {
                    TempData["SuccessMessage"] = "Number " + viewModel.UnitRequestNumber + " Rejected";
                }
                return RedirectToAction("Index", "ApprovalRequest");
            }

            return View();
        }
    }
}
