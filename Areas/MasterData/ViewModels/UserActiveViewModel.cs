using System.ComponentModel.DataAnnotations;

namespace PurchasingSystemDeveloper.Areas.MasterData.ViewModels
{
    public class UserActiveViewModel
    {
        public Guid UserActiveId { get; set; }
        public string UserActiveCode { get; set; }
        [Required(ErrorMessage = "Fullname is required !")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Identity Number is required !")]
        public string IdentityNumber { get; set; }
        [Required(ErrorMessage = "Department is required !")]
        public Guid? DepartmentId { get; set; }
        public string? Department { get; set; }
        [Required(ErrorMessage = "Position is required !")]
        public Guid? PositionId { get; set; }
        public string? Position { get; set; }
        [Required(ErrorMessage = "Place Of Birth is required !")]
        public string PlaceOfBirth { get; set; }
        [Required(ErrorMessage = "Date Of Birth is required !")]
        public DateTimeOffset DateOfBirth { get; set; } = DateTimeOffset.UtcNow;
        [Required(ErrorMessage = "Gender is required !")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Address is required !")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Handphone is required !")]
        public string Handphone { get; set; }
        [Required(ErrorMessage = "Email is required !")]
        public string Email { get; set; }
        public IFormFile? Foto { get; set; }
        public string? UserPhotoPath { get; set; }
        public bool IsActive { get; set; }
    }
}
