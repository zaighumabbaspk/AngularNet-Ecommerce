using eCommerce.Identity.API.Models;
using System.Security.Claims;

namespace eCommerce.Identity.API.Repositories;

public interface IUserManagement
{
    Task<bool> CreateUser(ApplicationUser user, string password);
    Task<bool> LoginUser(ApplicationUser user, string password);
    Task<ApplicationUser?> GetUserByEmail(string email);
    Task<ApplicationUser?> GetUserById(string id);
    Task<IEnumerable<ApplicationUser>> GetAllUsers();
    Task<int> RemoveUserByEmail(string email);
    Task<List<Claim>> GetUserByClaims(string email);
    Task<string> GeneratePasswordResetToken(ApplicationUser user);
    Task<bool> ResetPassword(ApplicationUser user, string token, string newPassword);
    Task<string> GenerateEmailConfirmationToken(ApplicationUser user);
    Task<bool> ConfirmEmail(ApplicationUser user, string token);
    Task<bool> IsEmailConfirmed(ApplicationUser user);
}
