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
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Transaction.Models;
using PurchasingSystemDeveloper.Areas.Transaction.Repositories;
using PurchasingSystemDeveloper.Areas.Transaction.ViewModels;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Route("Transaction/[Controller]/[Action]")]
    public class UnitRequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUnitRequestRepository _unitRequestRepository;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IApprovalRequestRepository _approvalRequestRepository;
        private readonly IUnitLocationRepository _unitLocationRepository;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;
        private readonly IWarehouseRequestRepository _warehouseRequestRepository;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;


        public UnitRequestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUnitRequestRepository UnitRequestRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IApprovalRequestRepository approvalRequestRepository,
            IUnitLocationRepository unitLocationRepository,
            IWarehouseLocationRepository warehouseLocationRepository,
            IWarehouseRequestRepository WarehouseRequestRepository,

            IHostingEnvironment hostingEnvironment,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _unitRequestRepository = UnitRequestRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _approvalRequestRepository = approvalRequestRepository;
            _unitLocationRepository = unitLocationRepository;
            _warehouseLocationRepository = warehouseLocationRepository;
            _warehouseRequestRepository = WarehouseRequestRepository;

            _hostingEnvironment = hostingEnvironment;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public JsonResult LoadProduk(Guid Id)
        {
            var produk = _applicationDbContext.Products.Include(p => p.Supplier).Include(s => s.Measurement).Include(d => d.Discount).Where(p => p.ProductId == Id).FirstOrDefault();
            return new JsonResult(produk);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Active = "UnitRequest";
            var data = _unitRequestRepository.GetAllUnitRequest();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "UnitRequest";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _unitRequestRepository.GetAllUnitRequest().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUnitRequest()
        {
            ViewBag.Active = "UnitRequest";

            _signInManager.IsSignedIn(User);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);

            UnitRequest UnitRequest = new UnitRequest()
            {
                UserAccessId = getUser.Id,
            };
            UnitRequest.UnitRequestDetails.Add(new UnitRequestDetail() { UnitRequestDetailId = Guid.NewGuid() });

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _unitRequestRepository.GetAllUnitRequest().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.UnitRequestNumber).FirstOrDefault();
            if (lastCode == null)
            {
                UnitRequest.UnitRequestNumber = "REQ" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.UnitRequestNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    UnitRequest.UnitRequestNumber = "REQ" + setDateNow + "0001";
                }
                else
                {
                    UnitRequest.UnitRequestNumber = "REQ" + setDateNow + (Convert.ToInt32(lastCode.UnitRequestNumber.Substring(9, lastCode.UnitRequestNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(UnitRequest);
        }


        [HttpPost]
        public async Task<IActionResult> CreateUnitRequest(UnitRequest model)
        {
            ViewBag.Active = "UnitRequest";

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _unitRequestRepository.GetAllUnitRequest().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.UnitRequestNumber).FirstOrDefault();
            if (lastCode == null)
            {
                model.UnitRequestNumber = "REQ" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.UnitRequestNumber.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    model.UnitRequestNumber = "REQ" + setDateNow + "0001";
                }
                else
                {
                    model.UnitRequestNumber = "REQ" + setDateNow + (Convert.ToInt32(lastCode.UnitRequestNumber.Substring(9, lastCode.UnitRequestNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var UnitRequest = new UnitRequest
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id), //Convert Guid to String
                    UnitRequestId = model.UnitRequestId,
                    UnitRequestNumber = model.UnitRequestNumber,
                    UserAccessId = getUser.Id,
                    UnitLocationId = model.UnitLocationId,
                    UnitRequestManagerId = model.UnitRequestManagerId,
                    WarehouseLocationId = model.WarehouseLocationId,
                    WarehouseApprovalId = model.WarehouseApprovalId,
                    QtyTotal = model.QtyTotal,
                    Status = model.Status,
                    Note = model.Note,
                };

                var ItemsList = new List<UnitRequestDetail>();

                foreach (var item in model.UnitRequestDetails)
                {
                    ItemsList.Add(new UnitRequestDetail
                    {
                        CreateDateTime = DateTime.Now,
                        CreateBy = new Guid(getUser.Id),
                        ProductNumber = item.ProductNumber,
                        ProductName = item.ProductName,
                        Measurement = item.Measurement,
                        Supplier = item.Supplier,
                        Qty = item.Qty,
                    });
                }

                UnitRequest.UnitRequestDetails = ItemsList;
                _unitRequestRepository.Tambah(UnitRequest);

                var approval = new ApprovalRequest
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    UnitRequestId = UnitRequest.UnitRequestId,
                    UnitRequestNumber = UnitRequest.UnitRequestNumber,
                    UserAccessId = getUser.Id.ToString(),
                    UnitLocationId = UnitRequest.UnitLocationId,
                    UnitRequestManagerId = UnitRequest.UnitRequestManagerId,
                    ApproveDate = DateTime.MinValue,
                    WarehouseApprovalId = UnitRequest.WarehouseApprovalId,
                    WarehouseApproveBy = "",
                    Status = UnitRequest.Status,
                    Note = UnitRequest.Note,
                };
                _approvalRequestRepository.Tambah(approval);

                TempData["SuccessMessage"] = "Number " + model.UnitRequestNumber + " Saved";
                return Json(new { redirectToUrl = Url.Action("Index", "UnitRequest") });
            }
            else
            {
                ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
                ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
                ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                TempData["WarningMessage"] = "Please, input all data !";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailUnitRequest(Guid Id)
        {
            ViewBag.Active = "UnitRequest";

            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);

            var UnitRequest = await _unitRequestRepository.GetUnitRequestById(Id);

            if (UnitRequest == null)
            {
                Response.StatusCode = 404;
                return View("UnitRequestNotFound", Id);
            }

            UnitRequest model = new UnitRequest
            {
                UnitRequestId = UnitRequest.UnitRequestId,
                UnitRequestNumber = UnitRequest.UnitRequestNumber,
                UserAccessId = UnitRequest.UserAccessId,
                UnitLocationId = UnitRequest.UnitLocationId,
                UnitRequestManagerId = UnitRequest.UnitRequestManagerId,
                WarehouseLocationId = UnitRequest.WarehouseLocationId,
                WarehouseApprovalId = UnitRequest.WarehouseApprovalId,
                QtyTotal = UnitRequest.QtyTotal,
                Status = UnitRequest.Status,
                Note = UnitRequest.Note,
            };

            var ItemsList = new List<UnitRequestDetail>();

            foreach (var item in UnitRequest.UnitRequestDetails)
            {
                ItemsList.Add(new UnitRequestDetail
                {
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Measurement = item.Measurement,
                    Supplier = item.Supplier,
                    Qty = item.Qty,
                });
            }

            model.UnitRequestDetails = ItemsList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DetailUnitRequest(UnitRequest model)
        {
            ViewBag.Active = "UnitRequest";

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                UnitRequest UnitRequest = _unitRequestRepository.GetAllUnitRequest().Where(p => p.UnitRequestNumber == model.UnitRequestNumber).FirstOrDefault();
                ApprovalRequest approval = _approvalRequestRepository.GetAllApprovalRequest().Where(p => p.UnitRequestNumber == model.UnitRequestNumber).FirstOrDefault();

                if (UnitRequest != null)
                {
                    if (approval != null)
                    {
                        UnitRequest.UpdateDateTime = DateTime.Now;
                        UnitRequest.UpdateBy = new Guid(getUser.Id);
                        UnitRequest.UnitLocationId = model.UnitLocationId;
                        UnitRequest.UnitRequestManagerId = model.UnitRequestManagerId;
                        UnitRequest.WarehouseLocationId = model.WarehouseLocationId;
                        UnitRequest.WarehouseApprovalId = model.WarehouseApprovalId;
                        UnitRequest.QtyTotal = model.QtyTotal;
                        UnitRequest.Note = model.Note;
                        UnitRequest.UnitRequestDetails = model.UnitRequestDetails;

                        approval.ApproveDate = DateTime.MinValue;
                        approval.WarehouseApprovalId = model.WarehouseApprovalId;
                        approval.Note = model.Note;

                        _applicationDbContext.Entry(approval).State = EntityState.Modified;
                        _unitRequestRepository.Update(UnitRequest);

                        TempData["SuccessMessage"] = "Number " + model.UnitRequestNumber + " Changes saved";
                        return Json(new { redirectToUrl = Url.Action("Index", "UnitRequest") });
                    }
                    else
                    {
                        ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
                        ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                        ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
                        ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                        ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                        TempData["WarningMessage"] = "Number " + model.UnitRequestNumber + " Not Found !!!";
                        return View(model);
                    }
                }
                else
                {
                    ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
                    ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                    ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
                    ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                    ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                    TempData["WarningMessage"] = "Number " + model.UnitRequestNumber + " Already exists !!!";
                    return View(model);
                }
            }
            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            TempData["WarningMessage"] = "Number " + model.UnitRequestNumber + " Failed saved";
            return Json(new { redirectToUrl = Url.Action("Index", "UnitRequest") });
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRequest(Guid Id)
        {
            ViewBag.Active = "UnitRequest";

            ViewBag.UnitLocation = new SelectList(await _unitLocationRepository.GetUnitLocations(), "UnitLocationId", "UnitLocationName", SortOrder.Ascending);
            ViewBag.RequestBy = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.WarehouseLocation = new SelectList(await _warehouseLocationRepository.GetWarehouseLocations(), "WarehouseLocationId", "WarehouseLocationName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);

            UnitRequest UnitRequest = _applicationDbContext.UnitRequests
                .Include(d => d.UnitRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.WarehouseApproval)
                .Include(p => p.WarehouseLocation)
                .Where(p => p.UnitRequestId == Id).FirstOrDefault();

            _signInManager.IsSignedIn(User);

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            WarehouseRequest wr = new WarehouseRequest();

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _warehouseRequestRepository.GetAllWarehouseRequest().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.WarehouseRequestNumber).FirstOrDefault();
            if (lastCode == null)
            {
                wr.WarehouseRequestNumber = "WRQ" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.WarehouseRequestNumber.Substring(3, 6);

                if (lastCodeTrim != setDateNow)
                {
                    wr.WarehouseRequestNumber = "WRQ" + setDateNow + "0001";
                }
                else
                {
                    wr.WarehouseRequestNumber = "WRQ" + setDateNow + (Convert.ToInt32(lastCode.WarehouseRequestNumber.Substring(9, lastCode.WarehouseRequestNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            ViewBag.WarehouseRequestNumber = wr.WarehouseRequestNumber;

            var getUnitReq = new UnitRequestViewModel()
            {
                UnitRequestId = UnitRequest.UnitRequestId,
                UnitRequestNumber = UnitRequest.UnitRequestNumber,
                UserAccessId = UnitRequest.UserAccessId,
                UnitLocationId = UnitRequest.UnitLocationId,
                UnitRequestManagerId = UnitRequest.UnitRequestManagerId,
                WarehouseLocationId = UnitRequest.WarehouseLocationId,
                WarehouseApprovalId = UnitRequest.WarehouseApprovalId,
                QtyTotal = UnitRequest.QtyTotal,
                Status = UnitRequest.Status,
                Note = UnitRequest.Note
            };

            var ItemsList = new List<UnitRequestDetail>();

            foreach (var item in UnitRequest.UnitRequestDetails)
            {
                ItemsList.Add(new UnitRequestDetail
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

            getUnitReq.UnitRequestDetails = ItemsList;
            return View(getUnitReq);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateRequest(UnitRequest model, UnitRequestViewModel vm)
        {
            ViewBag.Active = "UnitRequest";

            UnitRequest UnitRequest = await _unitRequestRepository.GetUnitRequestById(model.UnitRequestId);

            _signInManager.IsSignedIn(User);

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            string getWarehouseRequestNumber = Request.Form["WRNumber"];

            var updateUnitRequest = _unitRequestRepository.GetAllUnitRequest().Where(c => c.UnitRequestId == model.UnitRequestId).FirstOrDefault();
            if (updateUnitRequest != null)
            {
                {
                    updateUnitRequest.Status = "InProcess";
                };
                _applicationDbContext.Entry(updateUnitRequest).State = EntityState.Modified;
            }

            var newWarehouseRequest = new WarehouseRequest
            {
                CreateDateTime = DateTime.Now,
                CreateBy = new Guid(getUser.Id),
                UnitRequestId = UnitRequest.UnitRequestId,
                UnitRequestNumber = UnitRequest.UnitRequestNumber,
                UserAccessId = getUser.Id.ToString(),
                UnitLocationId = UnitRequest.UnitLocationId,
                UnitRequestManagerId = UnitRequest.UnitRequestManagerId,
                WarehouseLocationId = UnitRequest.WarehouseLocationId,
                WarehouseApprovalId = UnitRequest.WarehouseApprovalId,
                Status = "Process",
                QtyTotal = UnitRequest.QtyTotal,
            };

            newWarehouseRequest.WarehouseRequestNumber = getWarehouseRequestNumber;

            var ItemsList = new List<WarehouseRequestDetail>();

            foreach (var item in UnitRequest.UnitRequestDetails)
            {
                ItemsList.Add(new WarehouseRequestDetail
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Supplier = item.Supplier,
                    Measurement = item.Measurement,
                    Qty = item.Qty,
                    Checked = item.Checked
                });
            }

            newWarehouseRequest.WarehouseRequestDetails = ItemsList;
            _warehouseRequestRepository.Tambah(newWarehouseRequest);

            TempData["SuccessMessage"] = "Number " + newWarehouseRequest.WarehouseRequestNumber + " Saved";
            return RedirectToAction("Index", "UnitRequest");
        }

        public async Task<IActionResult> PrintUnitRequest(Guid Id)
        {
            var unitRequest = await _unitRequestRepository.GetUnitRequestById(Id);

            var CreateDate = DateTime.Now.ToString("dd MMMM yyyy");
            var ReqNumber = unitRequest.UnitRequestNumber;
            var CreateBy = unitRequest.ApplicationUser.NamaUser;
            var HeadUnit = unitRequest.UnitRequestManager.FullName;
            var UnitLocation = unitRequest.UnitLocation.UnitLocationName;
            var HeadWarehouse = unitRequest.WarehouseApproval.FullName;
            var WarehouseLocation = unitRequest.WarehouseLocation.WarehouseLocationName;            
            var Note = unitRequest.Note;
            var QtyTotal = unitRequest.QtyTotal;

            WebReport web = new WebReport();
            var path = $"{_webHostEnvironment.WebRootPath}\\Reporting\\UnitRequest.frx";
            web.Report.Load(path);

            var msSqlDataConnection = new MsSqlDataConnection();
            msSqlDataConnection.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            var Conn = msSqlDataConnection.ConnectionString;

            web.Report.SetParameterValue("Conn", Conn);
            web.Report.SetParameterValue("UnitRequestId", Id.ToString());
            web.Report.SetParameterValue("ReqNumber", ReqNumber);
            web.Report.SetParameterValue("CreateDate", CreateDate);
            web.Report.SetParameterValue("CreateBy", CreateBy);
            web.Report.SetParameterValue("HeadUnit", HeadUnit);
            web.Report.SetParameterValue("UnitLocation", UnitLocation);
            web.Report.SetParameterValue("HeadWarehouse", HeadWarehouse);
            web.Report.SetParameterValue("WarehouseLocation", WarehouseLocation);
            web.Report.SetParameterValue("Note", Note);
            web.Report.SetParameterValue("QtyTotal", QtyTotal);

            web.Report.Prepare();
            Stream stream = new MemoryStream();
            web.Report.Export(new PDFSimpleExport(), stream);
            stream.Position = 0;
            return File(stream, "application/zip", (ReqNumber + ".pdf"));
        }
    }
}
