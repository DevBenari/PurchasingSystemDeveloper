using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.MasterData.Models
{
    [Table("MstUnitLocation", Schema = "dbo")]
    public class UnitLocation : UserActivity
    {
        [Key]
        public Guid UnitLocationId { get; set; }
        public string UnitLocationCode { get; set; }
        public string UnitLocationName { get; set; }
        public Guid? UnitManagerId { get; set; }
        public string? Address { get; set; }

        //Relationship        
        [ForeignKey("UnitManagerId")]
        public UserActive? UnitManager { get; set; }
    }
}
