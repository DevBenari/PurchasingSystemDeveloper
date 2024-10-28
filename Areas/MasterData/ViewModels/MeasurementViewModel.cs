namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class MeasurementViewModel
    {
        public Guid MeasurementId { get; set; }
        public string MeasurementCode { get; set; }
        public string MeasurementName { get; set; }
        public string? Note { get; set; }
    }
}
