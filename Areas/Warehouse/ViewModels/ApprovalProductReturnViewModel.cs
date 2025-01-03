namespace PurchasingSystemDeveloper.Areas.Warehouse.ViewModels
{
    public class ApprovalProductReturnViewModel
    {
        public Guid ApprovalProductReturnId { get; set; }
        public Guid? ProductReturnId { get; set; }
        public string ProductReturnNumber { get; set; }
        public string UserAccessId { get; set; } //Dibuat Oleh
        public Guid? UserApproveId { get; set; }
        public string ApproveBy { get; set; }
        public string? ApprovalTime { get; set; }
        public DateTimeOffset ApprovalDate { get; set; }
        public string? ApprovalStatusUser { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public string? Message { get; set; }
    }
}
