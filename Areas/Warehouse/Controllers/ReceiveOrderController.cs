using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Route("Warehouse/[Controller]/[Action]")]
    public class ReceiveOrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IReceiveOrderRepository _receiveOrderRepository;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;

        public ReceiveOrderController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IReceiveOrderRepository receiveOrderRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ITermOfPaymentRepository termOfPaymentRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IPurchaseRequestRepository purchaseRequestRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _receiveOrderRepository = receiveOrderRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _termOfPaymentRepository = termOfPaymentRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseRequestRepository = purchaseRequestRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
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
            return new JsonResult(podetail);
        }

        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:Warehouse/ReceiveOrder/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
                string encryptedPath = _protector.Protect(originalPath);

                // Hash GUID-like code (SHA256 truncated to 36 characters)
                string guidLikeCode = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(encryptedPath)))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Substring(0, 36);

                // Simpan mapping GUID-like code ke encryptedPath di penyimpanan sementara (misalnya, cache)
                _urlMappingService.InMemoryMapping[guidLikeCode] = encryptedPath;

                return Redirect("/" + guidLikeCode);
            }
            catch
            {
                // Jika enkripsi gagal, kembalikan view
                return Redirect(Request.Path);
            }            
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            ViewBag.Active = "ReceiveOrder";
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedFilter = filterOptions;

            // Format tanggal untuk input[type="date"]
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // Format tanggal untuk tampilan (Indonesia)
            ViewBag.StartDateReadable = startDate?.ToString("dd MMMM yyyy");
            ViewBag.EndDateReadable = endDate?.ToString("dd MMMM yyyy");

            // Normalisasi tanggal untuk mengabaikan waktu
            if (startDate.HasValue) startDate = startDate.Value.Date;
            if (endDate.HasValue) endDate = endDate.Value.Date.AddDays(1).AddTicks(-1); // Sampai akhir hari

            // Tentukan range tanggal berdasarkan filterOptions
            if (!string.IsNullOrEmpty(filterOptions))
            {
                (startDate, endDate) = GetDateRangeHelper.GetDateRange(filterOptions);
            }

            var data = await _receiveOrderRepository.GetAllReceiveOrderPageSize(searchTerm, page, pageSize, startDate, endDate);

            var model = new Pagination<ReceiveOrder>
            {
                Items = data.receiveOrders,
                TotalCount = data.totalCountReceiveOrders,
                PageSize = pageSize,
                CurrentPage = page,
            };

            // Sertakan semua parameter untuk pagination
            ViewBag.FilterOptions = filterOptions;
            ViewBag.StartDateParam = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDateParam = endDate?.ToString("yyyy-MM-dd");
            ViewBag.PageSize = pageSize;

            return View(model);
        }

        public IActionResult RedirectToCreate()
        {
            try
            {
                ViewBag.Active = "ReceiveOrder";
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Create:Warehouse/ReceiveOrder/CreateReceiveOrder";
                string encryptedPath = _protector.Protect(originalPath);

                // Hash GUID-like code (SHA256 truncated to 36 characters)
                string guidLikeCode = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(encryptedPath)))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Substring(0, 36);

                // Simpan mapping GUID-like code ke encryptedPath di penyimpanan sementara (misalnya, cache)
                _urlMappingService.InMemoryMapping[guidLikeCode] = encryptedPath;

                return Redirect("/" + guidLikeCode);
            }
            catch
            {
                // Jika enkripsi gagal, kembalikan view
                return Redirect(Request.Path);
            }            
        }

        [HttpGet]
        public async Task<IActionResult> CreateReceiveOrder(string poList)
        {
            ViewBag.Active = "ReceiveOrder";
            //Pembelian Pembelian = await _pembelianRepository.GetAllPembelian().Where(p => p.PembelianNumber == );

            _signInManager.IsSignedIn(User);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.POFilter = new SelectList(await _purchaseOrderRepository.GetPurchaseOrdersFilters(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);

            ReceiveOrder ReceiveOrder = new ReceiveOrder()
            {
                ReceiveById = getUser.Id,
            };
            //ReceiveOrder.ReceiveOrderDetails.Add(new ReceiveOrderDetail() { ReceiveOrderDetailId = Guid.NewGuid() });

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var lastCode = _receiveOrderRepository.GetAllReceiveOrder().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.ReceiveOrderNumber).FirstOrDefault();
            if (lastCode == null)
            {
                ReceiveOrder.ReceiveOrderNumber = "RO" + setDateNow + "0001";
            }
            else
            {
                var lastCodeTrim = lastCode.ReceiveOrderNumber.Substring(2, 6);

                if (lastCodeTrim != setDateNow)
                {
                    ReceiveOrder.ReceiveOrderNumber = "RO" + setDateNow + "0001";
                }
                else
                {
                    ReceiveOrder.ReceiveOrderNumber = "RO" + setDateNow + (Convert.ToInt32(lastCode.ReceiveOrderNumber.Substring(9, lastCode.ReceiveOrderNumber.Length - 9)) + 1).ToString("D4");
                }
            }

            return View(ReceiveOrder);
        }


        [HttpPost]
        public async Task<IActionResult> CreateReceiveOrder(ReceiveOrder model)
        {
            ViewBag.Active = "ReceiveOrder";
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.POFilter = new SelectList(await _purchaseOrderRepository.GetPurchaseOrdersFilters(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);

            var checkDuplicate = _receiveOrderRepository.GetAllReceiveOrder().Where(r => r.PurchaseOrderId == model.PurchaseOrderId).ToList();
            
            if (checkDuplicate.Count == 0) 
            {
                var dateNow = DateTimeOffset.Now;
                var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

                var lastCode = _receiveOrderRepository.GetAllReceiveOrder().Where(d => d.CreateDateTime.ToString("yyMMdd") == dateNow.ToString("yyMMdd")).OrderByDescending(k => k.ReceiveOrderNumber).FirstOrDefault();
                if (lastCode == null)
                {
                    model.ReceiveOrderNumber = "RO" + setDateNow + "0001";
                }
                else
                {
                    var lastCodeTrim = lastCode.ReceiveOrderNumber.Substring(2, 6);

                    if (lastCodeTrim != setDateNow)
                    {
                        model.ReceiveOrderNumber = "RO" + setDateNow + "0001";
                    }
                    else
                    {
                        model.ReceiveOrderNumber = "RO" + setDateNow + (Convert.ToInt32(lastCode.ReceiveOrderNumber.Substring(9, lastCode.ReceiveOrderNumber.Length - 9)) + 1).ToString("D4");
                    }
                }

                var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    var receiveOrder = new ReceiveOrder
                    {
                        CreateDateTime = DateTime.Now,
                        CreateBy = new Guid(getUser.Id), //Convert Guid to String
                        ReceiveOrderId = model.ReceiveOrderId,
                        ReceiveOrderNumber = model.ReceiveOrderNumber,
                        PurchaseOrderId = model.PurchaseOrderId,
                        ReceiveById = getUser.Id,
                        ShippingNumber = model.ShippingNumber,
                        DeliveryServiceName = model.DeliveryServiceName,
                        DeliveryDate = model.DeliveryDate,
                        WaybillNumber = model.WaybillNumber,
                        InvoiceNumber = model.InvoiceNumber,
                        Status = model.Status,
                        Note = model.Note,
                        ReceiveOrderDetails = model.ReceiveOrderDetails,
                    };

                    var updateStatusPO = _purchaseOrderRepository.GetAllPurchaseOrder().Where(c => c.PurchaseOrderId == model.PurchaseOrderId).FirstOrDefault();
                    if (updateStatusPO != null)
                    {
                        updateStatusPO.UpdateDateTime = DateTime.Now;
                        updateStatusPO.UpdateBy = new Guid(getUser.Id);
                        updateStatusPO.Status = model.ReceiveOrderNumber;

                        _applicationDbContext.Entry(updateStatusPO).State = EntityState.Modified;
                    }

                    foreach (var item in receiveOrder.ReceiveOrderDetails)
                    {
                        var updateProduk = _productRepository.GetAllProduct().Where(c => c.ProductCode == item.ProductNumber).FirstOrDefault();
                        if (updateProduk != null)
                        {
                            updateProduk.UpdateDateTime = DateTime.Now;
                            updateProduk.UpdateBy = new Guid(getUser.Id);
                            updateProduk.Stock = updateProduk.Stock + item.QtyReceive;

                            _applicationDbContext.Entry(updateProduk).State = EntityState.Modified;
                        }
                    }

                    _receiveOrderRepository.Tambah(receiveOrder);
                    TempData["SuccessMessage"] = "Number " + model.ReceiveOrderNumber + " Saved";
                    return Json(new { redirectToUrl = Url.Action("Index", "ReceiveOrder") });
                }
                else
                {
                    ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                    ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                    ViewBag.POFilter = new SelectList(await _purchaseOrderRepository.GetPurchaseOrdersFilters(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
                    TempData["WarningMessage"] = "There is still empty data!!!";
                    return View(model);
                }
            }
            else
            {
                ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
                ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
                ViewBag.POFilter = new SelectList(await _purchaseOrderRepository.GetPurchaseOrdersFilters(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
                TempData["WarningMessage"] = "Sorry, the PO number has been created!";
                return View(model);
            }
        }

        public IActionResult RedirectToDetail(Guid Id)
        {
            try
            {
                ViewBag.Active = "ReceiveOrder";
                // Enkripsi path URL untuk "Index"
                string originalPath = $"Detail:Warehouse/ReceiveOrder/DetailReceiveOrder/{Id}";
                string encryptedPath = _protector.Protect(originalPath);

                // Hash GUID-like code (SHA256 truncated to 36 characters)
                string guidLikeCode = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(encryptedPath)))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Substring(0, 36);

                // Simpan mapping GUID-like code ke encryptedPath di penyimpanan sementara (misalnya, cache)
                _urlMappingService.InMemoryMapping[guidLikeCode] = encryptedPath;

                return Redirect("/" + guidLikeCode);
            }
            catch
            {
                // Jika enkripsi gagal, kembalikan view
                return Redirect(Request.Path);
            }            
        }

        [HttpGet]
        public async Task<IActionResult> DetailReceiveOrder(Guid Id)
        {
            ViewBag.Active = "ReceiveOrder";
            ViewBag.PO = new SelectList(await _purchaseOrderRepository.GetPurchaseOrders(), "PurchaseOrderId", "PurchaseOrderNumber", SortOrder.Ascending);
            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);

            var receiveOrder = await _receiveOrderRepository.GetReceiveOrderById(Id);

            if (receiveOrder == null)
            {
                Response.StatusCode = 404;
                return View("ReceiveOrderNotFound", Id);
            }

            ReceiveOrder model = new ReceiveOrder
            {
                ReceiveOrderId = receiveOrder.ReceiveOrderId,
                ReceiveOrderNumber = receiveOrder.ReceiveOrderNumber,
                PurchaseOrderId = receiveOrder.PurchaseOrderId,
                ReceiveById = receiveOrder.ReceiveById,
                ShippingNumber = receiveOrder.ShippingNumber,
                DeliveryServiceName = receiveOrder.DeliveryServiceName,
                DeliveryDate = receiveOrder.DeliveryDate,
                WaybillNumber = receiveOrder.WaybillNumber,
                InvoiceNumber = receiveOrder.InvoiceNumber,
                Status = receiveOrder.Status,
                Note = receiveOrder.Note,
                ReceiveOrderDetails = receiveOrder.ReceiveOrderDetails,
            };
            return View(model);
        }
    }
}
