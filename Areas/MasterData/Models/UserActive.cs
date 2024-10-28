using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.MasterData.Models
{
    [Table("MstUserActive", Schema = "dbo")]
    public class UserActive : UserActivity
    {
        [Key]
        public Guid UserActiveId { get; set; }
        public string UserActiveCode { get; set; }
        public string FullName { get; set; }
        public string IdentityNumber { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? PositionId { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Handphone { get; set; }
        public string Email { get; set; }
        public string? Foto { get; set; }
        public bool IsActive { get; set; }

        //Relationship
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        [ForeignKey("PositionId")]
        public Position? Position { get; set; }
    }
}
