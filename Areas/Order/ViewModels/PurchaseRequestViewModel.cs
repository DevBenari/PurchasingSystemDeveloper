using PurchasingSystemDeveloper.Areas.Order.Models;
using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.Order.ViewModels
{
    public class PurchaseRequestViewModel
    {
        public Guid PurchaseRequestId { get; set; }
        public string PurchaseRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public int ExpiredDay { get; set; }
        public int RemainingDay { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        [Required(ErrorMessage = "Department is required !")]
        public Guid? Department1Id { get; set; }
        [Required(ErrorMessage = "Position is required !")]
        public Guid? Position1Id { get; set; }
        [Required(ErrorMessage = "User Approve is required !")]
        public Guid? UserApprove1Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        [Required(ErrorMessage = "Department is required !")]
        public Guid? Department2Id { get; set; }
        [Required(ErrorMessage = "Position is required !")]
        public Guid? Position2Id { get; set; }
        [Required(ErrorMessage = "User Approve is required !")]
        public Guid? UserApprove2Id { get; set; }
        public string? ApproveStatusUser2 { get; set; }
        [Required(ErrorMessage = "Department is required !")]
        public Guid? Department3Id { get; set; }
        [Required(ErrorMessage = "Position is required !")]
        public Guid? Position3Id { get; set; }
        [Required(ErrorMessage = "User Approve is required !")]
        public Guid? UserApprove3Id { get; set; }
        public string? ApproveStatusUser3 { get; set; }
        public Guid? TermOfPaymentId { get; set; }
        public string Status { get; set; }
        public int QtyTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Note { get; set; }
        public string? MessageApprove1 { get; set; }
        public string? MessageApprove2 { get; set; }
        public string? MessageApprove3 { get; set; }
        public List<PurchaseRequestDetail> PurchaseRequestDetails { get; set; } = new List<PurchaseRequestDetail>();
    }
}
