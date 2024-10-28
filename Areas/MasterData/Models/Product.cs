using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.MasterData.Models
{
    [Table("MstProduct", Schema = "dbo")]
    public class Product : UserActivity
    {
        [Key]
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }        
        public Guid? SupplierId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? MeasurementId { get; set; }
        public Guid? DiscountId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }
        public int? BufferStock { get; set; }
        public int? Stock { get; set; }
        public decimal? Cogs { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal RetailPrice { get; set; }
        public string? StorageLocation { get; set; }
        public string? RackNumber { get; set; }
        public string? Note { get; set; }

        //Relationship        
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        [ForeignKey("MeasurementId")]
        public Measurement? Measurement { get; set; }
        [ForeignKey("DiscountId")]
        public Discount? Discount { get; set; }
        [ForeignKey("WarehouseLocationId")]
        public WarehouseLocation? WarehouseLocation { get; set; }
    }
}
