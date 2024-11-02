using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Transaction.Models;

namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class ApprovalUnitRequestViewModel
    {
        public Guid ApprovalUnitRequestId { get; set; }
        public Guid? UnitRequestId { get; set; }
        public string UnitRequestNumber { get; set; }
        public string UserAccessId { get; set; } //Dibuat Oleh
        public Guid? UnitLocationId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public Guid? UserApproveId { get; set; }
        public string? UserApprove { get; set; }
        public string ApproveBy { get; set; }
        public string? ApprovalTime { get; set; }
        public DateTimeOffset ApprovalDate { get; set; }
        public string? ApprovalStatusUser { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public string? Message { get; set; }
        public int QtyTotal { get; set; }
        public List<UnitRequestDetail> UnitRequestDetails { get; set; }
    }
}
