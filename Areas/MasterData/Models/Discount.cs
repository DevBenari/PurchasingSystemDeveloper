using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.MasterData.Models
{
    [Table("MstDiscount", Schema = "dbo")]
    public class Discount : UserActivity
    {
        [Key]
        public Guid DiscountId { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountValue { get; set; }
        public string? Note { get; set; }
    }
}
