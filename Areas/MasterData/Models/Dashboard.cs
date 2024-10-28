using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Models;

namespace PurchasingSystemDeveloper.Areas.MasterData.Models
{
    public class Dashboard
    {
        public IEnumerable<ApplicationUser> UserOnlines { get; set; }
        public UserActiveViewModel UserActiveViewModels { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
