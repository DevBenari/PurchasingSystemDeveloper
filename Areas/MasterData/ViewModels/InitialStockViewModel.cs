using PurchasingSystemDeveloper.Areas.MasterData.Models;
using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class InitialStockViewModel
    {
        public Guid InitialStockId { get; set; }
        [Required(ErrorMessage = "Generate By is required !")]
        public string GenerateBy { get; set; }
        [Required(ErrorMessage = "Product is required !")]
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        [Required(ErrorMessage = "Supplier is required !")]
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        [Required(ErrorMessage = "Lead Time By is required !")]
        public Guid? LeadTimeId { get; set; }
        [Required(ErrorMessage = "Calculate Base On is required !")]
        public string CalculateBaseOn { get; set; }
        [Required(ErrorMessage = "Max Request is required !")]
        public int MaxRequest { get; set; }
        [Required(ErrorMessage = "Average Request is required !")]
        public int AverageRequest { get; set; }
    }
}
