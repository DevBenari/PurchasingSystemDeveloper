using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Report.Models
{
    [Table("RptClosingPurchaseOrder", Schema = "dbo")]
    public class ClosingPurchaseOrder : UserActivity
    {
        [Key]
        public Guid ClosingPurchaseOrderId { get; set; }
        public string ClosingPurchaseOrderNumber { get; set; }
        public string UserAccessId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalPo { get; set; }  // Jumlah PO
        public int TotalQty { get; set; }  // Total Qty
        public decimal GrandTotal { get; set; }
        public List<ClosingPurchaseOrderDetail> ClosingPurchaseOrderDetails { get; set; } = new List<ClosingPurchaseOrderDetail>();

        //Relationship
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }

    [Table("RptClosingPurchaseOrderDetail", Schema = "dbo")]
    public class ClosingPurchaseOrderDetail : UserActivity
    {
        [Key]
        public Guid ClosingPurchaseOrderDetailId { get; set; }
        public Guid? ClosingPurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string TermOfPaymentName { get; set; }
        public string Status { get; set; }
        public string SupplierName { get; set; }
        public int Qty { get; set; }
        public decimal TotalPrice { get; set; }

        //Relationship
        [ForeignKey("ClosingPurchaseOrderId")]
        public ClosingPurchaseOrder? ClosingPurchaseOrder { get; set; }
    }
}
