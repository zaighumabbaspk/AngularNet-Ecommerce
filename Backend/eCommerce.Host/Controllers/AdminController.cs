using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eCommerce.Domain.Services.Interfaces.Authentication;
using eCommerce.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using eCommerce.Domain.Entities.Identity;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserManagement _userManagement;
        private readonly IRoleManagement _roleManagement;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(IUserManagement userManagement, IRoleManagement roleManagement, UserManager<AppUser> userManager)
        {
            _userManagement = userManagement;
            _roleManagement = roleManagement;
            _userManager = userManager;
        }

        /// <summary>
        /// Make a user admin by email (temporary endpoint for initial setup)
        /// </summary>
        [HttpPost("make-admin/{email}")]
        public async Task<IActionResult> MakeUserAdmin(string email)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new ServiceResponse(false, "User not found"));
                }

                // Check if user is already admin
                var currentRole = await _roleManagement.GetUserRole(email);
                if (currentRole == "Admin")
                {
                    return Ok(new ServiceResponse(true, "User is already an admin"));
                }

                // Add to Admin role
                var result = await _roleManagement.AddUserToRole(user, "Admin");
                if (!result)
                {
                    return BadRequest(new ServiceResponse(false, "Failed to assign admin role"));
                }

                return Ok(new ServiceResponse(true, $"User {email} has been made an admin successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponse(false, $"An error occurred: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get user role by email
        /// </summary>
        [HttpGet("user-role/{email}")]
        public async Task<IActionResult> GetUserRole(string email)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new ServiceResponse(false, "User not found"));
                }

                var role = await _roleManagement.GetUserRole(email);
                return Ok(new { Email = email, Role = role ?? "No role assigned" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponse(false, $"An error occurred: {ex.Message}"));
            }
        }

        /// <summary>
        /// Debug endpoint to check user roles and database state
        /// </summary>
        [HttpGet("debug-user/{email}")]
        public async Task<IActionResult> DebugUser(string email)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found", Email = email });
                }

                // Get all roles for this user using UserManager directly
                var allRoles = await _userManager.GetRolesAsync(user);
                var primaryRole = await _roleManagement.GetUserRole(email);

                return Ok(new 
                { 
                    Email = email,
                    UserId = user.Id,
                    UserName = user.UserName,
                    PrimaryRole = primaryRole ?? "No primary role",
                    AllRoles = allRoles.ToList(),
                    RoleCount = allRoles.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Remove user from User role and ensure only Admin role
        /// </summary>
        [HttpPost("fix-admin-role/{email}")]
        public async Task<IActionResult> FixAdminRole(string email)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new ServiceResponse(false, "User not found"));
                }

                // Get current roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                
                // Remove from all current roles
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                // Add only Admin role
                var result = await _userManager.AddToRoleAsync(user, "Admin");
                
                if (!result.Succeeded)
                {
                    return BadRequest(new ServiceResponse(false, $"Failed to assign admin role: {string.Join(", ", result.Errors.Select(e => e.Description))}"));
                }

                // Verify the change
                var newRoles = await _userManager.GetRolesAsync(user);
                
                return Ok(new ServiceResponse(true, $"User {email} role fixed. Current roles: {string.Join(", ", newRoles)}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponse(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}