using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.MasterData.Models
{
    [Table("MstBank", Schema = "dbo")]
    public class Bank : UserActivity
    {
        [Key]
        public Guid BankId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string CardHolderName { get; set; }
        public string? Note { get; set; }
    }
}
