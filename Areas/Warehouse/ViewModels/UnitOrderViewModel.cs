using PurchasingSystemDeveloper.Areas.Warehouse.Models;

namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class UnitOrderViewModel
    {
        public Guid UnitOrderId { get; set; }
        public string UnitOrderNumber { get; set; }
        public Guid? UnitRequestId { get; set; }
        public string UnitRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public Guid? UnitLocationId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public Guid? UserApprove1Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        public string Status { get; set; }
        public int QtyTotal { get; set; }
        public string? Note { get; set; }
        public List<UnitOrderDetail> UnitOrderDetails { get; set; } = new List<UnitOrderDetail>();
    }
}
