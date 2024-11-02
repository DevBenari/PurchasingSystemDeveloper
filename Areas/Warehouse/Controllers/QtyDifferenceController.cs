using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Transaction.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Hubs;
using PurchasingSystemDeveloper.Models;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Route("Warehouse/[Controller]/[Action]")]
    public class QtyDifferenceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IQtyDifferenceRepository _QtyDifferenceRepository;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IApprovalQtyDifferenceRepository _approvalQtyDifferenceRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;


        public QtyDifferenceController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IQtyDifferenceRepository QtyDifferenceRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IPurchaseRequestRepository purchaseRequestRepository,
            IApprovalQtyDifferenceRepository approvalQtyDifferenceRepository,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository,
            IHubContext<ChatHub> hubContext,

            IHostingEnvironment hostingEnvironment,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _QtyDifferenceRepository = QtyDifferenceRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _approvalQtyDifferenceRepository = approvalQtyDifferenceRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _hubContext = hubContext;

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

        public JsonResult LoadPurchaseOrder(Guid Id)
        {
            var podetail = _applicationDbContext.PurchaseOrders
                .Include(p => p.PurchaseOrderDetails)
                .Where(p => p.PurchaseOrderId == Id).FirstOrDefault();
            return new JsonResult(podetail);
        }
        public JsonResult LoadPurchaseOrderDetail(Guid Id)
        {
            var podetail = _applicationDbContext.PurchaseOrderDetails
                .Where(p => p.PurchaseOrderDetailId == Id).FirstOrDefault();
            return new JsonResult(podetail);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Active = "QtyDifference";
            var data = _QtyDifferenceRepository.GetAllQtyDifference();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "QtyDifference";

            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _QtyDifferenceRepository.GetAllQtyDifference().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> CreateQtyDifference(string poList)
        {
            ViewBag.Active = "QtyDifference";

            //Pembelian Pembelian = await _pembelianRepository.GetAllPembelian().Where(p => p.PembelianNumber == );

            _signInManager.IsSignedIn(User);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.POFilter = new SelectList(await _purchaseOrderRepository.GetPurchaseOrdersFilters(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);

            QtyDifference QtyDifference = new QtyDifference()
            {
                UserAccessId = getUser.Id,
            }; ;            

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _QtyDifferenceRepository.GetAllQtyDifference().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.QtyDifferenceNumber).FirstOrDefault();
            if (lastCode == null)
            {
                QtyDifference.QtyDifferenceNumber = "QD" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.QtyDifferenceNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    QtyDifference.QtyDifferenceNumber = "QD" + setDateNow + "0001";
                }
                else
                {
                    QtyDifference.QtyDifferenceNumber = "QD" + setDateNow + (Convert.ToInt32(lastCode.QtyDifferenceNumber.Substring(9, lastCode.QtyDifferenceNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(QtyDifference);
        }


        [HttpPost]
        public async Task<IActionResult> CreateQtyDifference(QtyDifference model)
        {
            ViewBag.Active = "QtyDifference";            

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _QtyDifferenceRepository.GetAllQtyDifference().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.QtyDifferenceNumber).FirstOrDefault();
            if (lastCode == null)
            {
                model.QtyDifferenceNumber = "QD" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.QtyDifferenceNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    model.QtyDifferenceNumber = "QD" + setDateNow + "0001";
                }
                else
                {
                    model.QtyDifferenceNumber = "QD" + setDateNow + (Convert.ToInt32(lastCode.QtyDifferenceNumber.Substring(9, lastCode.QtyDifferenceNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var QtyDifference = new QtyDifference
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id), //Convert Guid to String
                    QtyDifferenceId = model.QtyDifferenceId,
                    QtyDifferenceNumber = model.QtyDifferenceNumber,
                    PurchaseOrderId = model.PurchaseOrderId,
                    PurchaseOrderNumber = model.PurchaseOrderNumber,
                    UserAccessId = getUser.Id,
                    Department1Id = model.Department1Id,
                    Position1Id = model.Position1Id,
                    UserApprove1Id = model.UserApprove1Id,
                    ApproveStatusUser1 = model.ApproveStatusUser1,
                    Department2Id = model.Department2Id,
                    Position2Id = model.Position2Id,
                    UserApprove2Id = model.UserApprove2Id,
                    ApproveStatusUser2 = model.ApproveStatusUser2,
                    Status = model.Status,
                    Note = model.Note,
                    MessageApprove1 = model.MessageApprove1,
                    MessageApprove2 = model.MessageApprove2,
                    QtyDifferenceDetails = model.QtyDifferenceDetails,
                };
               
                _QtyDifferenceRepository.Tambah(QtyDifference);

                var updateStatusPO = _purchaseOrderRepository.GetAllPurchaseOrder().Where(c => c.PurchaseOrderId == model.PurchaseOrderId).FirstOrDefault();
                if (updateStatusPO != null)
                {
                    updateStatusPO.UpdateDateTime = DateTime.Now;
                    updateStatusPO.UpdateBy = new Guid(getUser.Id);
                    updateStatusPO.Status = "Process Cancel";

                    _applicationDbContext.Entry(updateStatusPO).State = EntityState.Modified;
                }

                //Signal R
                //var data2 = _QtyDifferenceRepository.GetAllQtyDifference();
                //int totalKaryawan = data2.Count();
                //await _hubContext.Clients.All.SendAsync("UpdateDataCount", totalKaryawan);
                //End Signal R     

                if (model.UserApprove1Id != null)
                {
                    var approvalQtyDiffRequest = new ApprovalQtyDifference
                    {
                        CreateDateTime = DateTime.Now,
                        CreateBy = new Guid(getUser.Id), //Convert Guid to String
                        QtyDifferenceId = QtyDifference.QtyDifferenceId,
                        QtyDifferenceNumber = QtyDifference.QtyDifferenceNumber,
                        UserAccessId = getUser.Id.ToString(),
                        PurchaseOrderId = QtyDifference.PurchaseOrderId,
                        PurchaseOrderNumber = QtyDifference.PurchaseOrderNumber,
                        UserApproveId = QtyDifference.UserApprove1Id,
                        ApproveBy = "",
                        ApprovalTime = "",
                        ApprovalDate = DateTimeOffset.MinValue,
                        ApprovalStatusUser = "User1",
                        Status = QtyDifference.Status,
                        Note = QtyDifference.Note,
                        Message = QtyDifference.MessageApprove1
                    };
                    _approvalQtyDifferenceRepository.Tambah(approvalQtyDiffRequest);
                }

                if (model.UserApprove2Id != null)
                {
                    var approvalQtyDiffRequest = new ApprovalQtyDifference
                    {
                        CreateDateTime = DateTime.Now,
                        CreateBy = new Guid(getUser.Id), //Convert Guid to String
                        QtyDifferenceId = QtyDifference.QtyDifferenceId,
                        QtyDifferenceNumber = QtyDifference.QtyDifferenceNumber,
                        UserAccessId = getUser.Id.ToString(),
                        PurchaseOrderId = QtyDifference.PurchaseOrderId,
                        PurchaseOrderNumber = QtyDifference.PurchaseOrderNumber,
                        UserApproveId = QtyDifference.UserApprove1Id,
                        ApproveBy = "",
                        ApprovalTime = "",
                        ApprovalDate = DateTimeOffset.MinValue,
                        ApprovalStatusUser = "User2",
                        Status = QtyDifference.Status,
                        Note = QtyDifference.Note,
                        Message = QtyDifference.MessageApprove1
                    };
                    _approvalQtyDifferenceRepository.Tambah(approvalQtyDiffRequest);
                }

                TempData["SuccessMessage"] = "Number " + model.QtyDifferenceNumber + " Saved";
                return Json(new { redirectToUrl = Url.Action("Index", "QtyDifference") });
            }
            else
            {
                ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                ViewBag.POFilter = new SelectList(await _purchaseOrderRepository.GetPurchaseOrdersFilters(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
                ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
                ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);
                TempData["WarningMessage"] = "There is still empty data!!!";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailQtyDifference(Guid Id)
        {
            ViewBag.Active = "QtyDifference";

            ViewBag.PO = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.Department = new SelectList(await _departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName", SortOrder.Ascending);
            ViewBag.Position = new SelectList(await _positionRepository.GetPositions(), "PositionId", "PositionName", SortOrder.Ascending);

            var QtyDifference = await _QtyDifferenceRepository.GetQtyDifferenceById(Id);

            if (QtyDifference == null)
            {
                Response.StatusCode = 404;
                return View("QtyDifferenceNotFound", Id);
            }

            QtyDifference model = new QtyDifference
            {
                QtyDifferenceId = QtyDifference.QtyDifferenceId,
                QtyDifferenceNumber = QtyDifference.QtyDifferenceNumber,
                PurchaseOrderId = QtyDifference.PurchaseOrderId,
                PurchaseOrderNumber = QtyDifference.PurchaseOrderNumber,
                UserAccessId = QtyDifference.UserAccessId,
                Department1Id = QtyDifference.Department1Id,
                Position1Id = QtyDifference.Position1Id,
                UserApprove1Id = QtyDifference.UserApprove1Id,
                ApproveStatusUser1 = QtyDifference.ApproveStatusUser1,
                Department2Id = QtyDifference.Department2Id,
                Position2Id = QtyDifference.Position2Id,
                UserApprove2Id = QtyDifference.UserApprove2Id,
                ApproveStatusUser2 = QtyDifference.ApproveStatusUser2,
                Status = QtyDifference.Status,
                Note = QtyDifference.Note,
                MessageApprove1 = QtyDifference.MessageApprove1,
                MessageApprove2 = QtyDifference.MessageApprove2,
                QtyDifferenceDetails = QtyDifference.QtyDifferenceDetails,
            };
            return View(model);
        }

        public async Task<IActionResult> PrintQtyDifference(Guid Id)
        {
            var qtyDifference = await _QtyDifferenceRepository.GetQtyDifferenceById(Id);

            var CreateDate = DateTime.Now.ToString("dd MMMM yyyy");
            var QdNumber = qtyDifference.QtyDifferenceNumber;
            var PoNumber = qtyDifference.PurchaseOrder.PurchaseOrderNumber;
            //var HeadWarehouse = qtyDifference.HeadWarehouseManager.FullName;
            //var HeadPurchasing = qtyDifference.HeadPurchasingManager.FullName;
            //var CheckedBy = qtyDifference.ApplicationUser.NamaUser;
            var Note = qtyDifference.Note;

            WebReport web = new WebReport();
            var path = $"{_webHostEnvironment.WebRootPath}\\Reporting\\QtyDifference.frx";
            web.Report.Load(path);

            var msSqlDataConnection = new MsSqlDataConnection();
            msSqlDataConnection.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            var Conn = msSqlDataConnection.ConnectionString;

            web.Report.SetParameterValue("Conn", Conn);
            web.Report.SetParameterValue("QtyDifferenceId", Id.ToString());
            web.Report.SetParameterValue("QdNumber", QdNumber);
            web.Report.SetParameterValue("PoNumber", PoNumber);
            web.Report.SetParameterValue("CreateDate", CreateDate);
            //web.Report.SetParameterValue("HeadWarehouse", HeadWarehouse);
            //web.Report.SetParameterValue("HeadPurchasing", HeadPurchasing);
            //web.Report.SetParameterValue("CheckedBy", CheckedBy);
            web.Report.SetParameterValue("Note", Note);

            web.Report.Prepare();
            Stream stream = new MemoryStream();
            web.Report.Export(new PDFSimpleExport(), stream);
            stream.Position = 0;
            return File(stream, "application/zip", (QdNumber + ".pdf"));
        }
    }
}
