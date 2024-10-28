using PurchasingSystemDeveloper.Areas.Warehouse.Models;

namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class WarehouseRequestViewModel
    {
        public Guid WarehouseRequestId { get; set; }
        public string WarehouseRequestNumber { get; set; }
        public Guid? UnitRequestId { get; set; }
        public string UnitRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public Guid? UnitLocationId { get; set; }
        public Guid? UnitRequestManagerId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public Guid? WarehouseApprovalId { get; set; }
        public string Status { get; set; }
        public int QtyTotal { get; set; }
        public List<WarehouseRequestDetail> WarehouseRequestDetails { get; set; } = new List<WarehouseRequestDetail>();
    }
}
