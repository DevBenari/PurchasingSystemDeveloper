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
        public Guid? UnitRequestManagerId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public Guid? WarehouseApprovalId { get; set; }
        public int QtyTotal { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public List<UnitRequestDetail> UnitRequestDetails { get; set; } = new List<UnitRequestDetail>();

        //Relationship
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
        [ForeignKey("UnitLocationId")]
        public UnitLocation? UnitLocation { get; set; }
        [ForeignKey("UnitRequestManagerId")]
        public UserActive? UnitRequestManager { get; set; }
        [ForeignKey("WarehouseLocationId")]
        public WarehouseLocation? WarehouseLocation { get; set; }
        [ForeignKey("WarehouseApprovalId")]
        public UserActive? WarehouseApproval { get; set; }
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
