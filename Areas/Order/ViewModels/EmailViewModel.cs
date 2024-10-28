namespace PurchasingSystemDeveloper.Areas.Order.ViewModels
{
    public class EmailViewModel
    {
        public Guid? EmailId { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public IFormFile? Document { get; set; }
        //public string AttachmentFileName { get; set; }
    }
}
