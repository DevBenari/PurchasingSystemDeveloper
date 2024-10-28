namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class DepartmentViewModel
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string? Note { get; set; }
    }
}
