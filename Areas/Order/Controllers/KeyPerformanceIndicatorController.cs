using FastReport;
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
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.Active = "KeyPerformanceIndikator";
            var data = _purchaseRequestRepository.GetAllPurchaseRequest();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> PostData(Selected model)
        {
            var getUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getUserLogin.KodeUser).FirstOrDefault();
            var user = await _userManager.GetUserAsync(User);
            var userId = new Guid(user.Id);
            var change = model.FirstName;
            var current = "";
            var formatDate = "";

            if (change == null || change == "month")
            {
                DateTime dateTime = DateTime.Now;
                current = dateTime.ToString("MMMM yyyy");
                formatDate = "MMMM yyyy";
            }
            else
            {
                DateTime dateTime = DateTime.Now;
                current = dateTime.ToString("yyyy");
                formatDate = "yyyy";
            }

            var getAllApproval = _approvalRepository.GetAllApprovalById(getUserActive.UserActiveId).ToList();
            var getApproval = _approvalRepository.GetAllApproval().Where(a => a.UserApproveId == getUserActive.UserActiveId && a.CreateDateTime.ToString(formatDate) == current);
            var getPurchaseRequest = _purchaseRequestRepository.GetAllPurchaseRequest().Where(x => x.CreateDateTime.ToString(formatDate) == current);
            var getPurchaseOrder = _purchaseOrderRepository.GetAllPurchaseOrder().Where(a => a.UserApprove1Id == getUserActive.UserActiveId || a.UserApprove2Id == getUserActive.UserActiveId || a.UserApprove3Id == getUserActive.UserActiveId && a.CreateDateTime.ToString(formatDate) == current);
            var thisMonthPO = getPurchaseOrder.Count(x => x.Status == "In Order");
            var thisMonthPR = getPurchaseRequest.Count();
            var prCreated = getPurchaseRequest.Count(x => x.CreateBy == userId);
            var countApproval = getApproval.Count();
            var waiting_approval = getApproval.Count(x => x.Status == "Waiting Approval");
            var approved = getApproval.Count(x => x.Status == "Approve");
            var reject = getApproval.Count(x => x.Status == "Reject");
            var completed = getPurchaseOrder.Count(x => x.Status != "In Order");

            var beforeRemaining = _approvalRepository.GetChartBeforeExpired(getUserActive.UserActiveId).GroupBy(u => u.CreateDateTime.ToString(formatDate)).Select(y => new
            {
                Months = y.Key,
                BeforeExpired = y.Count()
            }).ToList();

            var moreThanExpired = _approvalRepository.GetChartMoreThanExpired(getUserActive.UserActiveId).GroupBy(u => u.CreateDateTime.ToString(formatDate)).Select(y => new
            {
                Months = y.Key,
                AfterExpired = y.Count()
            }).ToList();

            var kpiData = new
            {
                DataApproval = getApproval,
                DataPurchaseRequest = thisMonthPR,
                DataCreated = prCreated,
                DataCountApproval = countApproval,
                DataWaiting = waiting_approval,
                DataRejected = reject,
                DataApproved = approved,
                DataPurchaseOrder = thisMonthPO,
                DataCompleted = completed,
                AllData = getAllApproval,
                BeforeExpired = beforeRemaining,
                MoreThanExpired = moreThanExpired,
            };

            return Json(kpiData);
        }

        public async Task<IActionResult> KpiJson(Selected model)
        {
            var getUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getUserLogin.KodeUser).FirstOrDefault();
            var user = await _userManager.GetUserAsync(User);
            var userId = new Guid(user.Id);
            var change = model.FirstName;
            var current = "";
            var formatDate = "";

            if (change == null || change == "month")
            {
                DateTime dateTime = DateTime.Now;
                current = dateTime.ToString("MMMM yyyy");
                formatDate = "MMMM yyyy";
            }
            else
            {
                DateTime dateTime = DateTime.Now;
                current = dateTime.ToString("yyyy");
                formatDate = "yyyy";
            }

            var getApproval = _approvalRepository.GetAllApproval().Where(a => a.UserApproveId == getUserActive.UserActiveId && a.CreateDateTime.ToString(formatDate) == current);
            var getPurchaseOrder = _purchaseOrderRepository.GetAllPurchaseOrder().Where(a => a.UserApprove1Id == getUserActive.UserActiveId || a.UserApprove2Id == getUserActive.UserActiveId || a.UserApprove3Id == getUserActive.UserActiveId && a.CreateDateTime.ToString(formatDate) == current);
            var fiveStar = getApproval.Count(x => x.RemainingDay > 0);
            var fourStar = getApproval.Count(x => x.RemainingDay == 0);
            var threeStar = getPurchaseOrder.Count(x => x.Status != "In Order");
            var twoStar = getApproval.Count(x => x.RemainingDay == -14);
            var oneStar = getApproval.Count(x => x.RemainingDay < -30);
            var data = getApproval.Count();

            var kpiRate = new
            {
                Data = data,
                FiveStart = fiveStar,
                FourStar = fourStar,
                ThreeStar = threeStar,
                TwoStar = twoStar,
                OneStar = oneStar
            };
            return Json(kpiRate);
        }

            public async Task<IActionResult> ChartJson()
        {
            //ViewBag.Active("chartJson");
            // GET USER ACTIVE ID FROM MSTUSERACTIVE
            var getUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getUserLogin.KodeUser).FirstOrDefault();

            // GET CURRENT MONTH AND YEAR 
            DateTime dateTime = DateTime.Now;
            var current = dateTime.ToString("MMMM yyyy");

            // GET APPROVAL BY ID FROM ORDERAPPROVAL
            var getApproval = _approvalRepository.GetAllApprovalById(getUserActive.UserActiveId).ToList();
            //var groupByMonths = _approvalRepository.GetAllApprovalById(getUserActive.UserActiveId).GroupBy(u => u.CreateDateTime.ToString("MMMM yyyy")).Select(y => new
            //{
            //    Months = y.Key,
            //    Counts = y.Count()
            //}).ToList();

            var beforeRemaining = _approvalRepository.GetChartBeforeExpired(getUserActive.UserActiveId).GroupBy(u => u.CreateDateTime.ToString("MMMM yyyy")).Select(y => new
            {
                Months = y.Key,
                BeforeExpired = y.Count()
            }).ToList();


            //var onExpired = _approvalRepository.GetChartOnExpired(getUserActive.UserActiveId).GroupBy(u => u.CreateDateTime.ToString("MMMM yyyy")).Select(y => new
            //{
            //    Months = y.Key,
            //    Counts = y.Count()
            //}).ToList();

            var moreThanExpired = _approvalRepository.GetChartMoreThanExpired(getUserActive.UserActiveId).GroupBy(u => u.CreateDateTime.ToString("MMMM yyyy")).Select(y => new
            {
                Months = y.Key,
                AfterExpired = y.Count()
            }).ToList();

            var dataView = new
            {
                AllData = getApproval,
                BeforeExpired = beforeRemaining,
                //OnExpired = onExpired,
                MoreThanExpired = moreThanExpired,
            };

            return Json(dataView);
        }

    }
}
