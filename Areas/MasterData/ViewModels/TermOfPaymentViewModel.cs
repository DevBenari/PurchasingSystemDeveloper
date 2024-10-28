namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class TermOfPaymentViewModel
    {
        public Guid TermOfPaymentId { get; set; }
        public string TermOfPaymentCode { get; set; }
        public string TermOfPaymentName { get; set; }
        public string? Note { get; set; }
    }
}
