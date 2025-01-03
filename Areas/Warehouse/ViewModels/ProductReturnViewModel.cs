using PurchasingSystemDeveloper.Areas.Warehouse.Models;

namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class ProductReturnViewModel
    {
        public Guid ProductReturnId { get; set; }
        public DateTimeOffset ReturnDate { get; set; } = DateTimeOffset.UtcNow;
        public string ProductReturnNumber { get; set; }
        public string UserAccessId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public Guid? Department1Id { get; set; }
        public Guid? Position1Id { get; set; }
        public Guid? UserApprove1Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        public Guid? Department2Id { get; set; }
        public Guid? Position2Id { get; set; }
        public Guid? UserApprove2Id { get; set; }
        public string? ApproveStatusUser2 { get; set; }
        public Guid? Department3Id { get; set; }
        public Guid? Position3Id { get; set; }
        public Guid? UserApprove3Id { get; set; }
        public string? ApproveStatusUser3 { get; set; }
        public string Status { get; set; }
        public string ReasonForReturn { get; set; }
        public string? Note { get; set; }
        public string? MessageApprove1 { get; set; }
        public string? MessageApprove2 { get; set; }
        public string? MessageApprove3 { get; set; }
        public List<ProductReturnDetail> ProductReturnDetails { get; set; } = new List<ProductReturnDetail>();
    }
}
