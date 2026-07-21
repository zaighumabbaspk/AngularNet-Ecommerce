using eCommerce.Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Identity.API.Repositories;

public class UserManagement : IUserManagement
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManagement(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> CreateUser(ApplicationUser user, string password)
    {
        var existing = await _userManager.FindByEmailAsync(user.Email!);
        if (existing != null) return false;

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) return false;

        // Ensure default role exists and assign
        var defaultRole = "User";
        if (!await _roleManager.RoleExistsAsync(defaultRole))
            await _roleManager.CreateAsync(new IdentityRole(defaultRole));

        await _userManager.AddToRoleAsync(user, defaultRole);
        return true;
    }

    public async Task<bool> LoginUser(ApplicationUser user, string password)
    {
        var existing = await _userManager.FindByEmailAsync(user.Email!);
        if (existing == null) return false;
        return await _userManager.CheckPasswordAsync(existing, password);
    }

    public async Task<ApplicationUser?> GetUserByEmail(string email)
        => await _userManager.FindByEmailAsync(email);

    public async Task<ApplicationUser?> GetUserById(string id)
        => await _userManager.FindByIdAsync(id);

    public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        => await _userManager.Users.AsNoTracking().ToListAsync();

    public async Task<int> RemoveUserByEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return 0;
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? 1 : 0;
    }

    public async Task<List<Claim>> GetUserByClaims(string email)
    {
        var user = await GetUserByEmail(email);
        var roles = user == null ? new List<string>() : (await _userManager.GetRolesAsync(user)).ToList();
        var role = roles.FirstOrDefault() ?? string.Empty;

        var claims = new List<Claim>
        {
            new Claim("FullName", user?.FullName ?? string.Empty),
            new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user?.Id ?? string.Empty),
            new Claim(ClaimTypes.Role, role)
        };

        return claims;
    }

    public async Task<string> GeneratePasswordResetToken(ApplicationUser user)
        => await _userManager.GeneratePasswordResetTokenAsync(user);

    public async Task<bool> ResetPassword(ApplicationUser user, string token, string newPassword)
    {
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<string> GenerateEmailConfirmationToken(ApplicationUser user)
        => await _userManager.GenerateEmailConfirmationTokenAsync(user);

    public async Task<bool> ConfirmEmail(ApplicationUser user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<bool> IsEmailConfirmed(ApplicationUser user)
        => await _userManager.IsEmailConfirmedAsync(user);
}
