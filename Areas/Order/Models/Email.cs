using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.Order.Models
{
    [Table("OrdPurchaseEmail", Schema = "dbo")]
    public class Email : UserActivity
    {
        [Key]
        public Guid? EmailId { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string? Document { get; set; }
        //public string AttachmentFileName { get; set; }
    }
}
