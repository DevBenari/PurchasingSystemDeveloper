using PurchasingSystemDeveloper.Areas.Report.Models;

namespace PurchasingSystemDeveloper.Areas.Report.ViewModels
{
    public class ClosingPurchaseOrderViewModel
    {
        public Guid ClosingPoId { get; set; }
        public string ClosingPoNumber { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public int TotalPo { get; set; }  // Jumlah PO
        public int TotalQty { get; set; }  // Total Qty
        public decimal GrandTotal { get; set; }
        public List<ClosingPurchaseOrderDetail> ClosingPurchaseOrderDetails { get; set; } = new List<ClosingPurchaseOrderDetail>();
    }
}
