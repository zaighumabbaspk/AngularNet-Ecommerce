
using eCommerce.Domain.Entities.Identity;
using System.Threading.Tasks;

namespace eCommerce.Domain.Services.Interfaces.Authentication
{
    public interface IRoleManagement
    {
        Task<string?> GetUserRole(string userEmail);
        Task<bool> AddUserToRole(AppUser user, string roleName);
    }

}

