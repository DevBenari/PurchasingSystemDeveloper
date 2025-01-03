using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Order.Models
{
    [Table("OrdPurchaseOrder", Schema = "dbo")]
    public class PurchaseOrder : UserActivity
    {
        [Key]
        public Guid PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public Guid? PurchaseRequestId { get; set; }
        public string PurchaseRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public Guid? UserApprove1Id { get; set; }
        public Guid? UserApprove2Id { get; set; }
        public Guid? UserApprove3Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        public string? ApproveStatusUser2 { get; set; }
        public string? ApproveStatusUser3 { get; set; }
        public Guid? TermOfPaymentId { get; set; }
        public string Status { get; set; }
        public int QtyTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Note { get; set; }
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

        //Relationship
        [ForeignKey("PurchaseRequestId")]
        public PurchaseRequest? PurchaseRequest { get; set; }
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
        [ForeignKey("TermOfPaymentId")]
        public TermOfPayment? TermOfPayment { get; set; }
        [ForeignKey("UserApprove1Id")]
        public UserActive? UserApprove1 { get; set; }
        [ForeignKey("UserApprove2Id")]
        public UserActive? UserApprove2 { get; set; }
        [ForeignKey("UserApprove3Id")]
        public UserActive? UserApprove3 { get; set; }
    }

    [Table("OrdPurchaseOrderDetail", Schema = "dbo")]
    public class PurchaseOrderDetail : UserActivity
    {
        [Key]
        public Guid PurchaseOrderDetailId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Measurement { get; set; }
        public string Supplier { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public decimal SubTotal { get; set; }

        //Relationship
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}
