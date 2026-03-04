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
<<<<<<< Updated upstream
=======
        Task<string> GeneratePasswordResetToken(AppUser user);
        Task<bool> ResetPassword(AppUser user, string token, string newPassword);
        Task<string> GenerateEmailConfirmationToken(AppUser user);
        Task<bool> ConfirmEmail(AppUser user, string token);
        Task<bool> IsEmailConfirmed(AppUser user);
>>>>>>> Stashed changes
    }
}
