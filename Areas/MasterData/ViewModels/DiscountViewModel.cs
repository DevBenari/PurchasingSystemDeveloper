using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class DiscountViewModel
    {
        public Guid DiscountId { get; set; }
        public string DiscountCode { get; set; }
        [Required(ErrorMessage = "Discount Value is required !")]
        public int DiscountValue { get; set; }
        public string? Note { get; set; }
    }
}
