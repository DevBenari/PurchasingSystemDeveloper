namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class DiscountViewModel
    {
        public Guid DiscountId { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountValue { get; set; }
        public string? Note { get; set; }
    }
}
