using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class TermOfPaymentViewModel
    {
        public Guid TermOfPaymentId { get; set; }
        public string TermOfPaymentCode { get; set; }
        [Required(ErrorMessage = "Term Of Payment Name is required !")]
        public string TermOfPaymentName { get; set; }
        public string? Note { get; set; }
    }
}
