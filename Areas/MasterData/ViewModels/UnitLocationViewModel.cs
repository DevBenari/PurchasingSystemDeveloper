namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class UnitLocationViewModel
    {
        public Guid UnitLocationId { get; set; }
        public string UnitLocationCode { get; set; }
        public string UnitLocationName { get; set; }
        public Guid? UnitManagerId { get; set; }
        public string? Address { get; set; }
    }
}
