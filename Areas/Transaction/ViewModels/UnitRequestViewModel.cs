using PurchasingSystemDeveloper.Areas.Transaction.Models;

namespace PurchasingSystemDeveloper.Areas.Transaction.ViewModels
{
    public class UnitRequestViewModel
    {
        public Guid UnitRequestId { get; set; }
        public string UnitRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public Guid? UnitLocationId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public Guid? Department1Id { get; set; }
        public Guid? Position1Id { get; set; }
        public Guid? UserApprove1Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        public int QtyTotal { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public string? MessageApprove1 { get; set; }
        public List<UnitRequestDetail> UnitRequestDetails { get; set; } = new List<UnitRequestDetail>();
    }
}
