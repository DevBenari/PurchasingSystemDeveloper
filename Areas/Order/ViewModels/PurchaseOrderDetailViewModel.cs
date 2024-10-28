namespace PurchasingSystemDeveloper.Areas.Order.ViewModels
{
    public class PurchaseOrderDetailViewModel
    {
        public Guid PurchaseOrderDetailId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Measurement { get; set; }
        public string Principal { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public decimal SubTotal { get; set; }
        public bool Checked { get; set; }
    }
}
