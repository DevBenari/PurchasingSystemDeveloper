using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class CategoryViewModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryCode { get; set; }
        [Required(ErrorMessage = "Category Name is required !")]
        public string CategoryName { get; set; }
        public string? Note { get; set; }
    }
}
