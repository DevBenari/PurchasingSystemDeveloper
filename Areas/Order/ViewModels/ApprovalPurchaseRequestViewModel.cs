using PurchasingSystemDeveloper.Areas.Order.Models;

namespace PurchasingSystemDeveloper.Areas.Order.ViewModels
{
    public class ApprovalPurchaseRequestViewModel
    {
        public Guid ApprovalId { get; set; }
        public Guid? PurchaseRequestId { get; set; }
        public string PurchaseRequestNumber { get; set; }
        public string UserAccessId { get; set; } //Dibuat Oleh
        public int ExpiredDay { get; set; }
        public int RemainingDay { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public Guid? UserApproveId { get; set; }
        public string? UserApprove { get; set; }
        public string ApproveBy { get; set; }
        public string? ApprovalTime { get; set; }
        public DateTimeOffset ApprovalDate { get; set; }
        public string? ApprovalStatusUser { get; set; }
        public string Status { get; set; }
        public string? Message { get; set; }
        public string? Note { get; set; }
        public int QtyTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public List<PurchaseRequestDetail> PurchaseRequestDetails { get; set; }
    }
}
