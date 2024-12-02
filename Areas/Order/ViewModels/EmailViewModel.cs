using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.Order.ViewModels
{
    public class EmailViewModel
    {
        public Guid? EmailId { get; set; }
        [Required(ErrorMessage = "To is required !")]
        public string To { get; set; }
        [Required(ErrorMessage = "Subject is required !")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Message is required !")]
        public string Message { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "Document is required !")]
        public IFormFile? Document { get; set; }
    }
}
