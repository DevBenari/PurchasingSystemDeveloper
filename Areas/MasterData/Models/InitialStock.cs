using Microsoft.CodeAnalysis;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.MasterData.Models
{
    [Table("MstInitialStock", Schema = "dbo")]
    public class InitialStock : UserActivity
    {
        [Key]
        public Guid InitialStockId { get; set; }
        public string GenerateBy { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public Guid? LeadTimeId { get; set; }
        public string CalculateBaseOn { get; set; }
        public int MaxRequest { get; set; }
        public int AverageRequest { get; set; }

        //Relationship        
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
        [ForeignKey("LeadTimeId")]
        public LeadTime? LeadTime { get; set; }
    }
}