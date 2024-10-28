using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;

namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class QtyDifferenceViewModel
    {
        public Guid QtyDifferenceId { get; set; }
        public string QtyDifferenceNumber { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string UserAccessId { get; set; } //Dibuat Oleh
        public Guid? Department1Id { get; set; }
        public Guid? Position1Id { get; set; }
        public Guid? UserApprove1Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        public Guid? Department2Id { get; set; }
        public Guid? Position2Id { get; set; }
        public Guid? UserApprove2Id { get; set; }
        public string? ApproveStatusUser2 { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public string? MessageApprove1 { get; set; }
        public string? MessageApprove2 { get; set; }
        public List<QtyDifferenceDetail> QtyDifferenceDetails { get; set; } = new List<QtyDifferenceDetail>();
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }
}
