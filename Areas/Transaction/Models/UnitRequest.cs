using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Transaction.Models
{
    [Table("TscUnitRequest", Schema = "dbo")]
    public class UnitRequest : UserActivity
    {
        [Key]
        public Guid UnitRequestId { get; set; }
        public string UnitRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public Guid? UnitLocationId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public Guid? Department1Id { get; set; }
        public Guid? Position1Id { get; set; }
        public Guid? UserApprove1Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        public int QtyTotal { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public string? MessageApprove1 { get; set; }
        public List<UnitRequestDetail> UnitRequestDetails { get; set; } = new List<UnitRequestDetail>();

        //Relationship
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
        [ForeignKey("UnitLocationId")]
        public UnitLocation? UnitLocation { get; set; }
        [ForeignKey("WarehouseLocationId")]
        public WarehouseLocation? WarehouseLocation { get; set; }
        [ForeignKey("Department1Id")]
        public Department? Department1 { get; set; }
        [ForeignKey("Position1Id")]
        public Position? Position1 { get; set; }
        [ForeignKey("UserApprove1Id")]
        public UserActive? UserApprove1 { get; set; }
    }

    [Table("TscUnitRequestDetail", Schema = "dbo")]
    public class UnitRequestDetail : UserActivity
    {
        [Key]
        public Guid UnitRequestDetailId { get; set; }
        public Guid? UnitRequestId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Measurement { get; set; }
        public string Supplier { get; set; }
        public int Qty { get; set; }
        public bool Checked { get; set; }

        //Relationship
        [ForeignKey("UnitRequestId")]
        public UnitRequest? UnitRequest { get; set; }
    }
}
