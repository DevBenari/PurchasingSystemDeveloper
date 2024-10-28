namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class SupplierViewModel
    {
        public Guid SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public Guid? LeadTimeId { get; set; }
        public string Address { get; set; }
        public string Handphone { get; set; }
        public string Email { get; set; }
        public string? Note { get; set; }
        public bool IsPKS { get; set; }
        public bool IsActive { get; set; }
    }
}
