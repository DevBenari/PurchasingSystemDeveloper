using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Order.ViewModels;
using PurchasingSystemDeveloper.Areas.Warehouse.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemDeveloper.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("Order/[Controller]/[Action]")]
    public class PurchaseOrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IQtyDifferenceRepository _qtyDifferenceRepository;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;


        public PurchaseOrderController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IPurchaseRequestRepository purchaseRequestRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IApprovalRepository approvalRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IQtyDifferenceRepository qtyDifferenceRepository,

            IHostingEnvironment hostingEnvironment,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _purchaseRequestRepository = purchaseRequestRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _approvalRepository = approvalRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _qtyDifferenceRepository = qtyDifferenceRepository;

            _hostingEnvironment = hostingEnvironment;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public JsonResult LoadProduk(Guid Id)
        {
            var produk = _applicationDbContext.Products.Include(p => p.Supplier).Include(s => s.Measurement).Include(d => d.Discount).Where(p => p.ProductId == Id).FirstOrDefault();
            return new JsonResult(produk);
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
            var qtyDiff = _applicationDbContext.QtyDifferences.Where(q => q.PurchaseOrderId == podetail.PurchaseOrderId).FirstOrDefault();
            var qtyDiffDetail = _applicationDbContext.QtyDifferenceDetails
                .Where(d => d.QtyDifferenceId == qtyDiff.QtyDifferenceId && d.ProductNumber == podetail.ProductNumber).FirstOrDefault();
            return new JsonResult(qtyDiffDetail);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Active = "PurchaseOrder";
            var data = _purchaseOrderRepository.GetAllPurchaseOrder();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "PurchaseOrder";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _purchaseOrderRepository.GetAllPurchaseOrder().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }        

        [HttpGet]
        public async Task<IActionResult> DetailPurchaseOrder(Guid Id)
        {
            ViewBag.Active = "PurchaseOrder";

            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);
            ViewBag.Pr = new SelectList(await _purchaseRequestRepository.GetPurchaseRequests(), "PurchaseRequestId", "PurchaseRequestNumber", SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);

            var purchaseOrder = await _purchaseOrderRepository.GetPurchaseOrderById(Id);

            if (purchaseOrder == null)
            {
                Response.StatusCode = 404;
                return View("PurchaseOrderNotFound", Id);
            }

            PurchaseOrder model = new PurchaseOrder
            {
                PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
                PurchaseRequestId = purchaseOrder.PurchaseRequestId,
                PurchaseRequestNumber = purchaseOrder.PurchaseRequestNumber,
                UserAccessId = purchaseOrder.UserAccessId,
                UserApprove1Id = purchaseOrder.UserApprove1Id,
                UserApprove2Id = purchaseOrder.UserApprove2Id,
                UserApprove3Id = purchaseOrder.UserApprove3Id,
                TermOfPaymentId = purchaseOrder.TermOfPaymentId,
                //DueDate = purchaseOrder.DueDate,
                Status = purchaseOrder.Status,
                QtyTotal = purchaseOrder.QtyTotal,
                GrandTotal = Math.Truncate(purchaseOrder.GrandTotal),
                Note = purchaseOrder.Note
            };

            var ItemsList = new List<PurchaseOrderDetail>();

            foreach (var item in purchaseOrder.PurchaseOrderDetails)
            {
                ItemsList.Add(new PurchaseOrderDetail
                {
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Supplier = item.Supplier,
                    Measurement = item.Measurement,
                    Qty = item.Qty,
                    Price = Math.Truncate(item.Price),
                    Discount = item.Discount,
                    SubTotal = Math.Truncate(item.SubTotal)
                });
            }

            model.PurchaseOrderDetails = ItemsList;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateNewPo(Guid Id)
        {
            ViewBag.Active = "PurchaseOrder";

            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);

            PurchaseOrder purchaseOrder = _applicationDbContext.PurchaseOrders
                .Include(d => d.PurchaseOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(a1 => a1.UserApprove1)
                .Include(a2 => a2.UserApprove2)
                .Include(a3 => a3.UserApprove3)
                .Include(p => p.TermOfPayment)
                .Where(p => p.PurchaseOrderId == Id).FirstOrDefault();

            _signInManager.IsSignedIn(User);

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            PurchaseOrder po = new PurchaseOrder();

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _purchaseOrderRepository.GetAllPurchaseOrder().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.PurchaseOrderNumber).FirstOrDefault();
            if (lastCode == null)
            {
                po.PurchaseOrderNumber = "PO" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.PurchaseOrderNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    po.PurchaseOrderNumber = "PO" + setDateNow + "0001";
                }
                else
                {
                    po.PurchaseOrderNumber = "PO" + setDateNow + (Convert.ToInt32(lastCode.PurchaseOrderNumber.Substring(9, lastCode.PurchaseOrderNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            ViewBag.PurchaseOrderNumber = po.PurchaseOrderNumber;

            var getQtyDiff = _qtyDifferenceRepository.GetAllQtyDifference().Where(po => po.PurchaseOrderId == purchaseOrder.PurchaseOrderId).FirstOrDefault();

            var getPo = new PurchaseOrder()
            {
                PurchaseRequestId = purchaseOrder.PurchaseRequestId,
                PurchaseRequestNumber = purchaseOrder.PurchaseRequestNumber,
                PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                ExpiredDate = purchaseOrder.ExpiredDate,
                UserAccessId = purchaseOrder.UserAccessId,
                UserApprove1Id = purchaseOrder.UserApprove1Id,
                UserApprove2Id = purchaseOrder.UserApprove2Id,
                UserApprove3Id = purchaseOrder.UserApprove3Id,
                ApproveStatusUser1 = purchaseOrder.ApproveStatusUser1,
                ApproveStatusUser2 = purchaseOrder.ApproveStatusUser2,
                ApproveStatusUser3 = purchaseOrder.ApproveStatusUser3,
                TermOfPaymentId = purchaseOrder.TermOfPaymentId,
                Status = purchaseOrder.Status,
                QtyTotal = purchaseOrder.QtyTotal,
                GrandTotal = Math.Truncate(purchaseOrder.GrandTotal),
                Note = "Reference from PO Number " + purchaseOrder.PurchaseOrderNumber,
            };

            var ItemsList = new List<PurchaseOrderDetail>();

            foreach (var item in purchaseOrder.PurchaseOrderDetails)
            {
                if (purchaseOrder.PurchaseOrderDetails.Count != 1)
                {
                    foreach (var itemQtyDiff in getQtyDiff.QtyDifferenceDetails)
                    {
                        if (itemQtyDiff.ProductName == item.ProductName && itemQtyDiff.QtyReceive == item.Qty)
                        {
                            ItemsList.Add(new PurchaseOrderDetail
                            {
                                CreateDateTime = DateTime.Now,
                                CreateBy = new Guid(getUser.Id),
                                ProductNumber = item.ProductNumber,
                                ProductName = item.ProductName,
                                Supplier = item.Supplier,
                                Measurement = item.Measurement,
                                Qty = item.Qty,
                                Price = Math.Truncate(item.Price),
                                Discount = item.Discount,
                                SubTotal = Math.Truncate(item.SubTotal)
                            });
                        }
                    }
                }
                else {
                    ItemsList.Add(new PurchaseOrderDetail
                    {
                        CreateDateTime = DateTime.Now,
                        CreateBy = new Guid(getUser.Id),
                        ProductNumber = item.ProductNumber,
                        ProductName = item.ProductName,
                        Supplier = item.Supplier,
                        Measurement = item.Measurement,
                        Qty = item.Qty,
                        Price = Math.Truncate(item.Price),
                        Discount = item.Discount,
                        SubTotal = Math.Truncate(item.SubTotal)
                    });
                }
            }

            getPo.PurchaseOrderDetails = ItemsList;
            return View(getPo);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateNewPo(PurchaseOrder model)
        {
            ViewBag.Active = "PurchaseOrder";

            if (ModelState.IsValid)
            {
                //PurchaseOrder purchaseOrder = await _purchaseOrderRepository.GetPurchaseOrderByIdNoTracking(model.PurchaseOrderId);

                _signInManager.IsSignedIn(User);

                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                //string getPurchaseOrderNumber = Request.Form["PONumber"];

                var updatePurchaseRequest = _purchaseRequestRepository.GetAllPurchaseRequest().Where(c => c.PurchaseRequestId == model.PurchaseRequestId).FirstOrDefault();
                if (updatePurchaseRequest != null)
                {
                    updatePurchaseRequest.Status = model.PurchaseOrderNumber;
                    _applicationDbContext.Entry(updatePurchaseRequest).State = EntityState.Modified;
                }

                var updateStatusPo = _purchaseOrderRepository.GetAllPurchaseOrder().Where(p => p.PurchaseRequestId == model.PurchaseRequestId).FirstOrDefault();
                if (updateStatusPo != null)
                {
                    updateStatusPo.Status = "Cancelled";
                    _applicationDbContext.Entry(updateStatusPo).State = EntityState.Modified;
                }

                var newPurchaseOrder = new PurchaseOrder
                {
                    CreateDateTime = DateTime.Now,
                    CreateBy = new Guid(getUser.Id),
                    PurchaseRequestId = model.PurchaseRequestId,
                    PurchaseRequestNumber = model.PurchaseRequestNumber,
                    ExpiredDate = model.ExpiredDate,
                    UserAccessId = getUser.Id.ToString(),
                    UserApprove1Id = model.UserApprove1Id,
                    UserApprove2Id = model.UserApprove2Id,
                    UserApprove3Id = model.UserApprove3Id,
                    ApproveStatusUser1 = model.ApproveStatusUser1,
                    ApproveStatusUser2 = model.ApproveStatusUser2,
                    ApproveStatusUser3 = model.ApproveStatusUser3,
                    TermOfPaymentId = model.TermOfPaymentId,
                    Status = "In Order",
                    QtyTotal = model.QtyTotal,
                    GrandTotal = Math.Truncate(model.GrandTotal),
                    Note = model.Note
                };

                newPurchaseOrder.PurchaseOrderNumber = model.PurchaseOrderNumber;

                var ItemsList = new List<PurchaseOrderDetail>();

                foreach (var item in model.PurchaseOrderDetails)
                {
                    ItemsList.Add(new PurchaseOrderDetail
                    {
                        CreateDateTime = DateTime.Now,
                        CreateBy = new Guid(getUser.Id),
                        ProductNumber = item.ProductNumber,
                        ProductName = item.ProductName,
                        Supplier = item.Supplier,
                        Measurement = item.Measurement,
                        Qty = item.Qty,
                        Price = Math.Truncate(item.Price),
                        Discount = item.Discount,
                        SubTotal = Math.Truncate(item.SubTotal)
                    });
                }

                newPurchaseOrder.PurchaseOrderDetails = ItemsList;

                _purchaseOrderRepository.Tambah(newPurchaseOrder);

                TempData["SuccessMessage"] = "Number " + newPurchaseOrder.PurchaseOrderNumber + " Saved";
                return Json(new { redirectToUrl = Url.Action("Index", "PurchaseOrder") });
            }

            return View();
        }

        public async Task<IActionResult> PrintPurchaseOrder(Guid Id)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetPurchaseOrderById(Id);

            var CreateDate = purchaseOrder.CreateDateTime.ToString("dd MMMM yyyy");
            var PoNumber = purchaseOrder.PurchaseOrderNumber;
            var CreateBy = purchaseOrder.ApplicationUser.NamaUser;
            var UserApprove1 = purchaseOrder.UserApprove1.FullName;
            var UserApprove2 = purchaseOrder.UserApprove2.FullName;
            var UserApprove3 = purchaseOrder.UserApprove3.FullName;
            var TermOfPayment = purchaseOrder.TermOfPayment.TermOfPaymentName;
            //var DueDate = purchaseOrder.DueDate;
            var Note = purchaseOrder.Note;
            var GrandTotal = purchaseOrder.GrandTotal;
            var Tax = (GrandTotal / 100) * 11;
            var GrandTotalAfterTax = (GrandTotal + Tax);

            WebReport web = new WebReport();
            var path = $"{_webHostEnvironment.WebRootPath}\\Reporting\\PurchaseOrder.frx";
            web.Report.Load(path);

            var msSqlDataConnection = new MsSqlDataConnection();
            msSqlDataConnection.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            var Conn = msSqlDataConnection.ConnectionString;

            web.Report.SetParameterValue("Conn", Conn);
            web.Report.SetParameterValue("PurchaseOrderId", Id.ToString());
            web.Report.SetParameterValue("PoNumber", PoNumber);
            web.Report.SetParameterValue("CreateDate", CreateDate);
            web.Report.SetParameterValue("CreateBy", CreateBy);
            web.Report.SetParameterValue("UserApprove1", UserApprove1);
            web.Report.SetParameterValue("UserApprove2", UserApprove2);
            web.Report.SetParameterValue("UserApprove3", UserApprove3);
            web.Report.SetParameterValue("TermOfPayment", TermOfPayment);
            web.Report.SetParameterValue("Note", Note);
            web.Report.SetParameterValue("GrandTotal", GrandTotal);
            web.Report.SetParameterValue("Tax", Tax);
            web.Report.SetParameterValue("GrandTotalAfterTax", GrandTotalAfterTax);

            web.Report.Prepare();
            Stream stream = new MemoryStream();
            web.Report.Export(new PDFSimpleExport(), stream);
            stream.Position = 0;
            return File(stream, "application/zip", (PoNumber + ".pdf"));
        }
    }
}
