using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Transaction.Models;

namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class ApprovalRequestViewModel
    {
        public Guid ApprovalRequestId { get; set; }
        public Guid? UnitRequestId { get; set; }
        public string UnitRequestNumber { get; set; }
        public string UserAccessId { get; set; } //Dibuat Oleh
        public Guid? UnitLocationId { get; set; }
        public Guid? UnitRequestManagerId { get; set; }
        public DateTime ApproveDate { get; set; }
        public Guid? WarehouseApprovalId { get; set; } //Mengetahui        
        public string WarehouseApproveBy { get; set; } //Disetujui Oleh
        public int QtyTotal { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public List<UnitRequestDetail> UnitRequestDetails { get; set; }
    }
}
