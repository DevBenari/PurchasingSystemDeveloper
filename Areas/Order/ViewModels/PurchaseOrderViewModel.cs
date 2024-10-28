using PurchasingSystemDeveloper.Areas.Order.Models;

namespace PurchasingSystemDeveloper.Areas.Order.ViewModels
{
    public class PurchaseOrderViewModel
    {
        public Guid PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public Guid? PurchaseRequestId { get; set; }
        public string PurchaseRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public Guid? UserApprove1Id { get; set; }
        public Guid? UserApprove2Id { get; set; }
        public Guid? UserApprove3Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        public string? ApproveStatusUser2 { get; set; }
        public string? ApproveStatusUser3 { get; set; }
        public Guid? TermOfPaymentId { get; set; }
        public Guid? DueDateId { get; set; }
        public string Status { get; set; }
        public int QtyTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Note { get; set; }
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }
}
