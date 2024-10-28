using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;

namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class ReceiveOrderViewModel
    {
        public Guid ReceiveOrderId { get; set; }
        public string ReceiveOrderNumber { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string ReceiveById { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public List<ReceiveOrderDetail> ReceiveOrderDetails { get; set; } = new List<ReceiveOrderDetail>();
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }
}
