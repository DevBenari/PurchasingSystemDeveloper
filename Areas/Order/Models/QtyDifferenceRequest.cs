using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Order.Models
{
    [Table("OrdQtyDifferenceRequest", Schema = "dbo")]
    public class QtyDifferenceRequest : UserActivity
    {
        [Key]
        public Guid QtyDifferenceRequestId { get; set; }
        public Guid QtyDifferenceId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string UserAccessId { get; set; }
        public DateTime QtyDifferenceApproveDate { get; set; }
        public string QtyDifferenceApproveBy { get; set; }
        public Guid? HeadWarehouseManagerId { get; set; }
        public Guid? HeadPurchasingManagerId { get; set; }        
        public string Status { get; set; }
        public string? Note { get; set; }
        
        //Relationship
        [ForeignKey("QtyDifferenceId")]
        public QtyDifference? QtyDifference { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }
        [ForeignKey("HeadWarehouseManagerId")]
        public UserActive? HeadWarehouseManager { get; set; }
        [ForeignKey("HeadPurchasingManagerId")]
        public UserActive? HeadPurchasingManager { get; set; }
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }   
}
