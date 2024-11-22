using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class WarehouseLocationViewModel
    {
        public Guid WarehouseLocationId { get; set; }
        public string WarehouseLocationCode { get; set; }
        [Required(ErrorMessage = "Fullname is required !")]
        public string WarehouseLocationName { get; set; }
        [Required(ErrorMessage = "Warehouse Manager is required !")]
        public Guid? WarehouseManagerId { get; set; }
        public string? Address { get; set; }
    }
}
