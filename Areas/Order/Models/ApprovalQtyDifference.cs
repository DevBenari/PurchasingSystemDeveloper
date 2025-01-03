using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Order.Models
{
    [Table("OrdApprovalQtyDifference", Schema = "dbo")]
    public class ApprovalQtyDifference : UserActivity
    {
        [Key]
        public Guid ApprovalQtyDifferenceId { get; set; }
        public Guid? QtyDifferenceId { get; set; }
        public string QtyDifferenceNumber { get; set; }
        public string UserAccessId { get; set; } //Dibuat Oleh
        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }        
        public Guid? UserApproveId { get; set; }
        public string ApproveBy { get; set; }
        public string? ApprovalTime { get; set; }
        public DateTimeOffset ApprovalDate { get; set; }
        public string? ApprovalStatusUser { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public string? Message { get; set; }

        //Relationship
        [ForeignKey("QtyDifferenceId")]
        public QtyDifference? QtyDifference { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }
        [ForeignKey("UserApproveId")]
        public UserActive? UserApprove { get; set; }
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
