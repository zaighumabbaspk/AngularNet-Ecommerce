
using eCommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using eCommerce.Infrastructure.Data;
using eCommerce.Domain.Services.Interfaces.Authentication;




namespace eCommerce.Application.Services.Implementation.Authentication
{
    public class UserManagement : IUserManagement
    {
        private readonly IRoleManagement _roleManagement;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public UserManagement(IRoleManagement roleManagement, UserManager<AppUser> userManager, AppDbContext context)
        {
            _roleManagement = roleManagement;
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> CreateUser(AppUser user, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email!);
            if (existingUser != null)
                return false;

            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsers()
            => await _context.Users.AsNoTracking().ToListAsync();

        public async Task<AppUser> GetUserByEmail(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<AppUser> GetUserById(string id)
            => await _userManager.FindByIdAsync(id);

        public async Task<bool> LoginUser(AppUser user, string password)
        {
            var existingUser = await GetUserByEmail(user.Email!);
            if (existingUser is null)
                return false;

            string? roleName = await _roleManagement.GetUserRole(existingUser.Email!);
            if (string.IsNullOrEmpty(roleName))
                return false;

            return await _userManager.CheckPasswordAsync(existingUser, password);
        }

        public async Task<int> RemoveUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return 0;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? 1 : 0;
        }

        public async Task<List<Claim>> GetUserByClaims(string email)
        {
            var user = await GetUserByEmail(email);
            string? role = await _roleManagement.GetUserRole(user!.Email!);

            var claims = new List<Claim>
            {
                new Claim("FullName", user.FullName),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, role!)
            };
            return claims;
        }
    }
}