namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class UnitOrderDetailViewModel
    {
        public Guid WarehouseRequestDetailId { get; set; }
        public Guid? WarehouseRequestId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Measurement { get; set; }
        public string Principal { get; set; }
        public int Qty { get; set; }
        public int QtySent { get; set; }
        public bool Checked { get; set; }
    }
}
