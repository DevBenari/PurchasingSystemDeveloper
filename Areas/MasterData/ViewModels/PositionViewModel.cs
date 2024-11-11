using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class PositionViewModel
    {
        public Guid PositionId { get; set; }
        public string PositionCode { get; set; }
        [Required(ErrorMessage = "Position Name is required !")]
        public string PositionName { get; set; }
        [Required(ErrorMessage = "Department Name is required !")]
        public Guid? DepartmentId { get; set; }
    }
}
