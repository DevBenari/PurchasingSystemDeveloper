using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace PurchasingSystemDeveloper.Areas.Report.Controllers
{
    [Area("Report")]
    [Route("Report/[Controller]/[Action]")]
    public class ReportPurchaseOrderController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;

        public ReportPurchaseOrderController(
            ApplicationDbContext applicationDbContext,
            IPurchaseOrderRepository purchaseOrderRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService
        )
        {           
            _applicationDbContext = applicationDbContext;
            _purchaseOrderRepository = purchaseOrderRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
        }

        public IActionResult RedirectToIndex(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 50)
        {
            try
            {
                // Format tanggal tanpa waktu
                string startDateString = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
                string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:Report/ReportPurchaseOrder/Index?filterOptions={filterOptions}&searchTerm={searchTerm}&startDate={startDateString}&endDate={endDateString}&page={page}&pageSize={pageSize}";
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

        public async Task<IActionResult> Index(int? month, int? year, string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 50)
        {
            ViewBag.Active = "Report";
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

            // Default ke bulan dan tahun saat ini jika tidak ada input
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var selectedMonth = month ?? currentMonth;
            var selectedYear = year ?? currentYear;

            var data = await _purchaseOrderRepository.GetAllPurchaseOrderPageSize(searchTerm, page, pageSize, startDate, endDate);

            // Hitung Grand Total dari semua data yang sesuai dengan filter
            var filteredOrders = data.purchaseOrders
                .Where(po => po.CreateDateTime.Month == selectedMonth && po.CreateDateTime.Year == selectedYear);

            // Gabungkan data yang sudah di-filter dalam pagination
            var ordersWithSupplier = filteredOrders.Select(po => new PurchaseOrderWithDetailSupplier
            {
                CreateDateTime = po.CreateDateTime,
                CreateBy = po.CreateBy,
                PurchaseOrderId = po.PurchaseOrderId,                
                PurchaseOrderNumber = po.PurchaseOrderNumber,
                TermOfPayment = po.TermOfPayment.TermOfPaymentName,
                QtyTotal = po.QtyTotal,
                GrandTotal = po.GrandTotal,
                SupplierName = po.PurchaseOrderDetails
                    .Select(pod => pod.Supplier).FirstOrDefault()                    
            }).ToList();

            var model = new Pagination<PurchaseOrderWithDetailSupplier>
            {
                Items = ordersWithSupplier,
                TotalCount = data.totalCountPurchaseOrders,
                PageSize = pageSize,
                CurrentPage = page,
            };            

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.Months = Enumerable.Range(1, 12)
                .Select(i => new { Value = i, Text = new DateTime(currentYear, i, 1).ToString("MMMM") })
                .ToList();
            ViewBag.Years = Enumerable.Range(currentYear - 10, 11).ToList(); // 10 tahun ke belakang

            ViewBag.GrandTotal = filteredOrders.Sum(o => o.GrandTotal);

            return View(model);
        }

        public async Task<IActionResult> ClosingPurchaseOrder()
        {
            return View();
        }

        public class PurchaseOrderWithDetailSupplier
        {            
            public DateTimeOffset CreateDateTime { get; set; }
            public Guid CreateBy { get; set; }
            public Guid PurchaseOrderId { get; set; }
            public string PurchaseOrderNumber { get; set; }
            public string TermOfPayment { get; set; }
            public int QtyTotal { get; set; }
            public decimal GrandTotal { get; set; }
            public string SupplierName { get; set; }  // Menyimpan nama supplier
        }
    }
}
