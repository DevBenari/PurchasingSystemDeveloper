using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Models
{
    [Table("WrhProductReturn", Schema = "dbo")]
    public class ProductReturn : UserActivity
    {
        [Key]
        public Guid ProductReturnId { get; set; }
        public DateTimeOffset ReturnDate { get; set; }
        public string ProductReturnNumber { get; set; }
        public string UserAccessId { get; set; }
        public string PurchaseOrderNumber { get; set; }
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
        public string Status { get; set; }
        public string ReasonForReturn { get; set; }
        public string? Note { get; set; }
        public string? MessageApprove1 { get; set; }
        public string? MessageApprove2 { get; set; }
        public string? MessageApprove3 { get; set; }
        public List<ProductReturnDetail> ProductReturnDetails { get; set; } = new List<ProductReturnDetail>();

        //Relationship
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
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

    [Table("WrhProductReturnDetail", Schema = "dbo")]
    public class ProductReturnDetail : UserActivity
    {
        [Key]
        public Guid ProductReturnDetailId { get; set; }
        public Guid? ProductReturnId { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Measurement { get; set; }
        public string WarehouseOrigin { get; set; }
        public string WarehouseExpired { get; set; }
        public string Supplier { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public decimal SubTotal { get; set; }

        //Relationship
        [ForeignKey("ProductReturnId")]
        public ProductReturn? ProductReturn { get; set; }
    }
}
