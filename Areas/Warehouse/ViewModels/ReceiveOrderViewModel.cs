using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class ReceiveOrderViewModel
    {
        public Guid ReceiveOrderId { get; set; }
        public string ReceiveOrderNumber { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string ReceiveById { get; set; }
        [Required(ErrorMessage = "Shipping Number is required !")]
        public string ShippingNumber { get; set; }
        [Required(ErrorMessage = "Delivery Service Name is required !")]
        public string DeliveryServiceName { get; set; }
        [Required(ErrorMessage = "Delivery Date is required !")]
        public string DeliveryDate { get; set; }
        [Required(ErrorMessage = "Waybill Number is required !")]
        public string WaybillNumber { get; set; }
        [Required(ErrorMessage = "Invoice Number is required !")]
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public List<ReceiveOrderDetail> ReceiveOrderDetails { get; set; } = new List<ReceiveOrderDetail>();
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }
}
