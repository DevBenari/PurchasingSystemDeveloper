using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.Transaction.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Models
{
    [Table("WrhUnitOrder", Schema = "dbo")]
    public class UnitOrder : UserActivity
    {
        [Key]
        public Guid UnitOrderId { get; set; }
        public string UnitOrderNumber { get; set; }
        public Guid? UnitRequestId { get; set; }
        public string UnitRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public Guid? UnitLocationId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public Guid? UserApprove1Id { get; set; }
        public string? ApproveStatusUser1 { get; set; }
        public string Status { get; set; }
        public int QtyTotal { get; set; }
        public string? Note { get; set; }
        public List<UnitOrderDetail> UnitOrderDetails { get; set; } = new List<UnitOrderDetail>();

        //Relationship
        [ForeignKey("UnitRequestId")]
        public UnitRequest? UnitRequest { get; set; }
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
        [ForeignKey("UnitLocationId")]
        public UnitLocation? UnitLocation { get; set; }        
        [ForeignKey("WarehouseLocationId")]
        public WarehouseLocation? WarehouseLocation { get; set; }
        [ForeignKey("UserApprove1Id")]
        public UserActive? UserApprove1 { get; set; }
    }

    [Table("WrhUnitOrderDetail", Schema = "dbo")]
    public class UnitOrderDetail : UserActivity
    {
        [Key]
        public Guid UnitOrderDetailId { get; set; }
        public Guid? UnitOrderId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Measurement { get; set; }
        public string Supplier { get; set; }
        public int Qty { get; set; }
        public int QtySent { get; set; }
        public bool Checked { get; set; }

        //Relationship
        [ForeignKey("UnitOrderId")]
        public UnitOrder? UnitOrder { get; set; }
    }
}
