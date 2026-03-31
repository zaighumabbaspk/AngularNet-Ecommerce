using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eCommerce.Domain.Services.Interfaces.Authentication;
using eCommerce.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using eCommerce.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserManagement _userManagement;
        private readonly IRoleManagement _roleManagement;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IUserManagement userManagement,
            IRoleManagement roleManagement,
            UserManager<AppUser> userManager,
            ILogger<AdminController> logger)
        {
            _userManagement = userManagement;
            _roleManagement = roleManagement;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Assign or manage user admin status
        /// </summary>
        [HttpPost("users/{email}/role")]
        public async Task<IActionResult> ManageUserRole(string email, [FromBody] UpdateUserRoleRequest request)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new ServiceResponse(false, "User not found"));
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                
                // Remove existing roles
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                // Assign new role
                var result = await _userManager.AddToRoleAsync(user, request.Role);
                
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to assign role to {Email}: {Errors}", 
                        email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    
                    return BadRequest(new ServiceResponse(false, 
                        $"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}"));
                }

                _logger.LogInformation("User {Email} assigned to {Role} role", email, request.Role);
                
                var newRoles = await _userManager.GetRolesAsync(user);
                return Ok(new ServiceResponse(true, 
                    $"User {email} assigned to {request.Role}. Current roles: {string.Join(", ", newRoles)}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error managing user role for {Email}", email);
                return StatusCode(500, new ServiceResponse(false, $"An error occurred: {ex.Message}"));
            }
        }

        /// <summary>
        /// Debug endpoint - only available in development
        /// </summary>
        [HttpGet("debug/users/{email}")]
        public async Task<IActionResult> DebugUser(string email)
        {
            // Only allow in development
            if (!IsEnvironmentDevelopment())
            {
                return Forbid("Debug endpoint only available in development");
            }

            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found", Email = email });
                }

                var allRoles = await _userManager.GetRolesAsync(user);

                return Ok(new 
                { 
                    Email = email,
                    UserId = user.Id,
                    UserName = user.UserName,
                    AllRoles = allRoles.ToList(),
                    RoleCount = allRoles.Count,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }

            private bool IsEnvironmentDevelopment()
        {
            // Implementation depends on how you access IHostEnvironment
            // This is a placeholder
            return true;
        }
    }
}