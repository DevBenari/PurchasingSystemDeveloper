using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.MasterData.Models
{
    [Table("MstWarehouseLocation", Schema = "dbo")]
    public class WarehouseLocation : UserActivity
    {
        [Key]
        public Guid WarehouseLocationId { get; set; }
        public string WarehouseLocationCode { get; set; }
        public string WarehouseLocationName { get; set; }
        public Guid? WarehouseManagerId { get; set; }
        public string? Address { get; set; }

        //Relationship        
        [ForeignKey("WarehouseManagerId")]
        public UserActive? WarehouseManager { get; set; }
    }
}
