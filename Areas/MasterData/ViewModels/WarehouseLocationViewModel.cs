namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class WarehouseLocationViewModel
    {
        public Guid WarehouseLocationId { get; set; }
        public string WarehouseLocationCode { get; set; }
        public string WarehouseLocationName { get; set; }
        public Guid? WarehouseManagerId { get; set; }
        public string? Address { get; set; }
    }
}
