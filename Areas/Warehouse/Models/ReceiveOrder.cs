using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Models
{
    [Table("WrhReceiveOrder", Schema = "dbo")]
    public class ReceiveOrder : UserActivity
    {
        [Key]
        public Guid ReceiveOrderId { get; set; }
        public string ReceiveOrderNumber { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string ReceiveById { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }        
        public List<ReceiveOrderDetail> ReceiveOrderDetails { get; set; } = new List<ReceiveOrderDetail>();
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

        //Relationship
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }
        [ForeignKey("ReceiveById")]
        public ApplicationUser? ApplicationUser { get; set; }
    }

    [Table("WrhReceiveOrderDetail", Schema = "dbo")]
    public class ReceiveOrderDetail : UserActivity
    {
        [Key]
        public Guid ReceivedOrderDetailId { get; set; }
        public Guid? ReceiveOrderId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Measure { get; set; }
        public int QtyOrder { get; set; }
        public int QtyReceive { get; set; }

        //Relationship        
        [ForeignKey("ReceiveOrderId")]
        public ReceiveOrder? ReceiveOrder { get; set; }
    }
}
