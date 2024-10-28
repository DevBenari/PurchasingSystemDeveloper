namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class UserActiveViewModel
    {
        public Guid UserActiveId { get; set; }
        public string UserActiveCode { get; set; }
        public string FullName { get; set; }
        public string IdentityNumber { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? Department { get; set; }
        public Guid? PositionId { get; set; }
        public string? Position { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTimeOffset DateOfBirth { get; set; } = DateTimeOffset.UtcNow;
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Handphone { get; set; }
        public string Email { get; set; }
        public IFormFile? Foto { get; set; }
        public string? UserPhotoPath { get; set; }
        public bool IsActive { get; set; }
    }
}
