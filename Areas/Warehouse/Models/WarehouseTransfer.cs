using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.Transaction.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Models
{
    [Table("WrhWarehouseTransfer", Schema = "dbo")]
    public class WarehouseTransfer : UserActivity
    {
        [Key]
        public Guid WarehouseTransferId { get; set; }
        public string WarehouseTransferNumber { get; set; }
        public Guid? WarehouseRequestId { get; set; }
        public string WarehouseRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public Guid? UnitLocationId { get; set; }
        public Guid? UnitRequestManagerId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public Guid? WarehouseApprovalId { get; set; }
        public string Status { get; set; }
        public int QtyTotal { get; set; }
        public List<WarehouseTransferDetail> WarehouseTransferDetails { get; set; } = new List<WarehouseTransferDetail>();

        //Relationship
        [ForeignKey("WarehouseRequestId")]
        public WarehouseRequest? WarehouseRequest { get; set; }
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

    [Table("WrhWarehouseTransferDetail", Schema = "dbo")]
    public class WarehouseTransferDetail : UserActivity
    {
        [Key]
        public Guid WarehouseTransferDetailId { get; set; }
        public Guid? WarehouseTransferId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Measurement { get; set; }
        public string Supplier { get; set; }
        public int Qty { get; set; }
        public int QtySent { get; set; }
        public bool Checked { get; set; }

        //Relationship
        [ForeignKey("WarehouseTransferId")]
        public WarehouseTransfer? WarehouseTransfer { get; set; }
    }
}
