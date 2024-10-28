using Microsoft.AspNetCore.Identity;

namespace PurchasingSystemDeveloper.Repositories
{
    public interface IRoleRepository
    {
        ICollection<IdentityRole> GetRoles();
    }
}
