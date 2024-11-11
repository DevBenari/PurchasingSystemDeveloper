using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class SupplierViewModel
    {
        public Guid SupplierId { get; set; }
        public string SupplierCode { get; set; }
        [Required(ErrorMessage = "Supplier Name is required !")]
        public string SupplierName { get; set; }
        [Required(ErrorMessage = "Lead Time is required !")]
        public Guid? LeadTimeId { get; set; }
        [Required(ErrorMessage = "Address is required !")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Handphone is required !")]
        public string Handphone { get; set; }
        [Required(ErrorMessage = "Email is required !")]
        public string Email { get; set; }
        public string? Note { get; set; }
        public bool IsPKS { get; set; }
        public bool IsActive { get; set; }
    }
}
