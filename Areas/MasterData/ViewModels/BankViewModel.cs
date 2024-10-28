namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class BankViewModel
    {
        public Guid BankId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string CardHolderName { get; set; }
        public string? Note { get; set; }
    }
}
