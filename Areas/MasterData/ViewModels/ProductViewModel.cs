using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class ProductViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        [Required(ErrorMessage = "Sorry, please fill in!")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Sorry, please choose!")]
        public Guid? SupplierId { get; set; }
        [Required(ErrorMessage = "Sorry, please choose!")]
        public Guid? CategoryId { get; set; }
        [Required(ErrorMessage = "Sorry, please choose!")]
        public Guid? MeasurementId { get; set; }
        [Required(ErrorMessage = "Sorry, please choose!")]
        public Guid? DiscountId { get; set; }
        [Required(ErrorMessage = "Sorry, please choose!")]
        public Guid? WarehouseLocationId { get; set; }
        public int? MinStock { get; set; } = 0;
        public int? MaxStock { get; set; } = 0;
        public int? BufferStock { get; set; } = 0;
        public int? Stock { get; set; } = 0;
        public decimal? Cogs { get; set; } = 0;
        [Range(1, 1000000000000, ErrorMessage = "Sorry, please fill in > 0 !")]
        public decimal BuyPrice { get; set; }
        [Range(1, 1000000000000, ErrorMessage = "Sorry, please fill in > 0 !")]
        public decimal RetailPrice { get; set; }
        public string? StorageLocation { get; set; }
        public string? RackNumber { get; set; }
        public string? Note { get; set; }
    }
}
