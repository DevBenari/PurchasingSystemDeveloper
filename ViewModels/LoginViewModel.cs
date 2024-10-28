using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PurchasingSystemDeveloper.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email tidak boleh kosong")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Kata sandi tidak boleh kosong")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Ingatkan Saya")]
        public bool RememberMe { get; set; }
    }
}
