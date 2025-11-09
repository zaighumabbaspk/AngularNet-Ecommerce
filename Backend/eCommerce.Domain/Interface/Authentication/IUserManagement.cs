using eCommerce.Domain.Entities.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eCommerce.Domain.Services.Interfaces.Authentication
{
    public interface IUserManagement
    {
        Task<bool> CreateUser(AppUser user, string password);
        Task<bool> LoginUser(AppUser user, string password);
        Task<AppUser> GetUserByEmail(string email);
        Task<AppUser> GetUserById(string id);
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<int> RemoveUserByEmail(string email);
        Task<List<Claim>> GetUserByClaims(string email);
    }
}
