using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
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
using PurchasingSystemDeveloper.Areas.Transaction.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using System.Diagnostics.Metrics;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
namespace PurchasingSystemDeveloper.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("Order/[Controller]/[Action]")]
    public class KeyPerformanceIndicatorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IQtyDifferenceRequestRepository _qtyDifferenceRequestRepository;
        private readonly IUserActiveRepository _userActiveRepository;
        public KeyPerformanceIndicatorController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IApprovalRepository approvalRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IPurchaseRequestRepository purchaseRequestRepository,
            IQtyDifferenceRequestRepository qtyDifferenceRequestRepository,
            IUserActiveRepository userActiveRepository
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _approvalRepository = approvalRepository;   
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _qtyDifferenceRequestRepository = qtyDifferenceRequestRepository;
            _userActiveRepository = userActiveRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Active = "KeyPerformanceIndikator";
            var data = _purchaseRequestRepository.GetAllPurchaseRequest();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Json()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = new Guid(user.Id);
            var dataRequest = _purchaseRequestRepository.GetAllPurchaseRequest().ToList();
            var getLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUser = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getLogin.KodeUser).FirstOrDefault();
            var dataApproval = _approvalRepository.GetAllApproval().Where(x => x.UserApproveId == getUser.UserActiveId).ToList();
            var dataOrder  = _purchaseOrderRepository.GetAllPurchaseOrder().Where(x => x.UserApprove1Id == getUser.UserActiveId || x.UserApprove2Id == getUser.UserActiveId || x.UserApprove3Id == getUser.UserActiveId).ToList();   

            // Gunakan userId untuk melakukan sesuatu, misalnya:
            ViewBag.UserId = userId;
            var viewData = new
            {
                UserId = userId,
                DataRequest = dataRequest,
                DataApproval = dataApproval,
                DataOrder = dataOrder,
            };
            return Json(viewData);
        }
    }
}
