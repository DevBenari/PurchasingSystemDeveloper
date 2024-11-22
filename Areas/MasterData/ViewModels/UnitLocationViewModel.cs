using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class UnitLocationViewModel
    {
        public Guid UnitLocationId { get; set; }
        public string UnitLocationCode { get; set; }
        [Required(ErrorMessage = "Unit Location Name is required !")]
        public string UnitLocationName { get; set; }
        [Required(ErrorMessage = "Unit Manager is required !")]
        public Guid? UnitManagerId { get; set; }
        public string? Address { get; set; }
    }
}
