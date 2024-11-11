using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class DepartmentViewModel
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        [Required(ErrorMessage = "Department Name is required !")]
        public string DepartmentName { get; set; }
        public string? Note { get; set; }
    }
}
