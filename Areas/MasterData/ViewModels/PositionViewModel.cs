namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class PositionViewModel
    {
        public Guid PositionId { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
