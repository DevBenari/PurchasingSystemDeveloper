using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class LeadTimeViewModel
    {
        public Guid LeadTimeId { get; set; }
        public string LeadTimeCode { get; set; }
        [Required(ErrorMessage = "Lead Time Value is required !")]
        public int LeadTimeValue { get; set; }
    }
}
