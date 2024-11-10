using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class BankViewModel
    {
        public Guid BankId { get; set; }
        public string BankCode { get; set; }
        [Required(ErrorMessage = "Bank Name is required !")]
        public string BankName { get; set; }
        [Required(ErrorMessage = "Account Number is required !")]
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Card Holder Name is required !")]
        public string CardHolderName { get; set; }
        public string? Note { get; set; }
    }
}
