using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Models
{
    [Table("WrhApprovalProductReturn", Schema = "dbo")]
    public class ApprovalProductReturn : UserActivity
    {
        [Key]
        public Guid ApprovalProductReturnId { get; set; }
        public Guid? ProductReturnId { get; set; }
        public string ProductReturnNumber { get; set; }
        public string UserAccessId { get; set; } //Dibuat Oleh
        public Guid? UserApproveId { get; set; }
        public string ApproveBy { get; set; }
        public string? ApprovalTime { get; set; }
        public DateTimeOffset ApprovalDate { get; set; }
        public string? ApprovalStatusUser { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public string? Message { get; set; }

        //Relationship
        [ForeignKey("ProductReturnId")]
        public ProductReturn? ProductReturn { get; set; }
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
        [ForeignKey("UserApproveId")]
        public UserActive? UserApprove { get; set; }
    }
}
