using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Models
{
    [Table("WrhQtyDifference", Schema = "dbo")]
    public class QtyDifference : UserActivity
    {
        [Key]
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

        //Relationship
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }
        [ForeignKey("Department1Id")]
        public Department? Department1 { get; set; }
        [ForeignKey("Position1Id")]
        public Position? Position1 { get; set; }
        [ForeignKey("UserApprove1Id")]
        public UserActive? UserApprove1 { get; set; }
        [ForeignKey("Department2Id")]
        public Department? Department2 { get; set; }
        [ForeignKey("Position2Id")]
        public Position? Position2 { get; set; }
        [ForeignKey("UserApprove2Id")]
        public UserActive? UserApprove2 { get; set; }
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }

    [Table("WrhQtyDifferenceDetail", Schema = "dbo")]
    public class QtyDifferenceDetail : UserActivity
    {
        [Key]
        public Guid QtyDifferenceDetailId { get; set; }
        public Guid? QtyDifferenceId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Measurement { get; set; }
        public string Supplier { get; set; }
        public int QtyOrder { get; set; }
        public int QtyReceive { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }

        //Relationship        
        [ForeignKey("QtyDifferenceId")]
        public QtyDifference? QtyDifference { get; set; }
    }
}
