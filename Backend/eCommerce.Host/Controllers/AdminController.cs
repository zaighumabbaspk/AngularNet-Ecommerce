using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eCommerce.infrastructure.Repositories.Authentication;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IRoleManagement _roleManagement;
        private readonly IUserManagement _userManagement;

        public AdminController(IRoleManagement roleManagement, IUserManagement userManagement)
        {
            _roleManagement = roleManagement;
            _userManagement = userManagement;
        }

        [HttpPost("assign-admin-role/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignAdminRole(string email)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var result = await _roleManagement.AddUserToRole(user, "Admin");
                if (result)
                {
                    return Ok(new { message = "Admin role assigned successfully" });
                }

                return BadRequest(new { message = "Failed to assign admin role" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManagement.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("user-roles/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var roles = await _roleManagement.GetUserRoles(user);
                return Ok(new { email = user.Email, roles = roles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // Debug endpoint to fix admin role assignment
        [HttpGet("fix-admin-role/{email}")]
        public async Task<IActionResult> FixAdminRole(string email)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Remove user from User role first
                await _roleManagement.RemoveUserFromRole(user, "User");
                
                // Add user to Admin role
                var result = await _roleManagement.AddUserToRole(user, "Admin");
                
                if (result)
                {
                    return Ok(new { message = "Admin role fixed successfully" });
                }

                return BadRequest(new { message = "Failed to fix admin role" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}