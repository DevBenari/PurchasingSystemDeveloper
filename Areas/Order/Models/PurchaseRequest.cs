using Microsoft.VisualBasic;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Order.Models
{
    [Table("OrdPurchaseRequest", Schema = "dbo")]
    public class PurchaseRequest : UserActivity
    {
        [Key]
        public Guid PurchaseRequestId { get; set; }
        public string PurchaseRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public int ExpiredDay { get; set; }
        public int RemainingDay { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public Guid? Department1Id { get; set; }
        public Guid? Position1Id { get; set; }
        public Guid? UserApprove1Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        public Guid? Department2Id { get; set; }
        public Guid? Position2Id { get; set; }
        public Guid? UserApprove2Id { get; set; }
        public string? ApproveStatusUser2 { get; set; }
        public Guid? Department3Id { get; set; }
        public Guid? Position3Id { get; set; }
        public Guid? UserApprove3Id { get; set; }
        public string? ApproveStatusUser3 { get; set; }
        public Guid? TermOfPaymentId { get; set; }
        public string Status { get; set; }
        public int QtyTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Note { get; set; }
        public string? MessageApprove1 { get; set; }
        public string? MessageApprove2 { get; set; }
        public string? MessageApprove3 { get; set; }
        public List<PurchaseRequestDetail> PurchaseRequestDetails { get; set; } = new List<PurchaseRequestDetail>();

        //Relationship
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }     
        [ForeignKey("TermOfPaymentId")]
        public TermOfPayment? TermOfPayment { get; set; }
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
        [ForeignKey("Department3Id")]
        public Department? Department3 { get; set; }
        [ForeignKey("Position3Id")]
        public Position? Position3 { get; set; }
        [ForeignKey("UserApprove3Id")]
        public UserActive? UserApprove3 { get; set; }
    }

    [Table("OrdPurchaseRequestDetail", Schema = "dbo")]
    public class PurchaseRequestDetail : UserActivity
    {
        [Key]
        public Guid PurchaseRequestDetailId { get; set; }
        public Guid? PurchaseRequestId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }        
        public string Measurement { get; set; }
        public string Supplier { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public decimal SubTotal { get; set; }

        //Relationship
        [ForeignKey("PurchaseRequestId")]
        public PurchaseRequest? PurchaseRequest { get; set; }
    }
}
