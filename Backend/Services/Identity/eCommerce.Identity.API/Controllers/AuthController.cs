using eCommerce.Identity.API.Models;
using eCommerce.Identity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ITokenManagement _tokenManagement;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserManagement _userManagement;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IEmailService emailService,
        ITokenManagement tokenManagement,
        IUserManagement userManagement,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _tokenManagement = tokenManagement;
        _userManagement = userManagement;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ServiceResponse>> CreateUser([FromBody] RegisterRequest request)
    {
        try
        {
            // Create new user via repository/service
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userManagement.CreateUser(user, request.Password);
            if (!created)
            {
                return BadRequest(new ServiceResponse { Success = false, Message = "Email Already Exists or invalid data" });
            }

            // Retrieve created user and generate email confirmation token
            var createdUser = await _userManager.FindByEmailAsync(request.Email);
            var emailToken = await _userManagement.GenerateEmailConfirmationToken(createdUser!);

            // Create verification link
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
            var verificationLink = $"{frontendUrl}/verify-email?token={Uri.EscapeDataString(emailToken)}&email={Uri.EscapeDataString(request.Email)}";

            // Send verification email
            try
            {
                await _emailService.SendEmailVerificationAsync(request.Email, verificationLink);
            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Failed to send verification email to {Email}", request.Email);
            }

            _logger.LogInformation("User registered successfully: {Email}", request.Email);

            return Ok(new ServiceResponse
            {
                Success = true,
                Message = "Account created successfully! Please check your email to verify your account."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", request.Email);
            return StatusCode(500, new ServiceResponse
            {
                Success = false,
                Message = $"An error occurred during registration: {ex.Message}"
            });
        }
    }
   
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginUser([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid Email or Password"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid Email or Password"
                });
            }

            // Check email confirmation (skip in development)
            var isEmailConfirmed = await _userManagement.IsEmailConfirmed(user);
            if (!isEmailConfirmed && _configuration["ASPNETCORE_ENVIRONMENT"] != "Development")
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Please verify your email address before logging in. Check your inbox for the verification link."
                });
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT token
            var token = _tokenService.GenerateToken(user, roles);

            // Generate refresh token and persist via token management
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _tokenManagement.UpdateRefreshToken(user.Id, refreshToken);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login Successful",
                Token = token,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Email = user.Email
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user: {Email}", request.Email);
            return StatusCode(500, new LoginResponse
            {
                Success = false,
                Message = $"An unexpected error occurred: {ex.Message}"
            });
        }
    }

    [HttpGet("refreshToken/{refreshToken}")]
    public async Task<ActionResult<LoginResponse>> ReviveToken(string refreshToken)
    {
        try
        {
            // Validate refresh token exists in DB
            var isValid = await _tokenManagement.ValidateRefreshToken(refreshToken);
            if (!isValid)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid Refresh Token"
                });
            }
            var userId = await _tokenManagement.GetUserIdByRefreshToken(refreshToken);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new LoginResponse { Success = false, Message = "Invalid Refresh Token" });
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate new tokens
            var newJwtToken = _tokenService.GenerateToken(user, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Update refresh token in DB
            await _tokenManagement.UpdateRefreshToken(user.Id, newRefreshToken);

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
                Token = newJwtToken,
                RefreshToken = newRefreshToken,
                UserId = user.Id,
                Email = user.Email
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, new LoginResponse
            {
                Success = false,
                Message = "An error occurred while refreshing token"
            });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ServiceResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist for security reasons
                return Ok(new ServiceResponse
                {
                    Success = true,
                    Message = "If your email exists in our system, you will receive a password reset link."
                });
            }

            // Generate password reset token
            var resetToken = await _userManagement.GeneratePasswordResetToken(user);

            // Create reset link
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
            var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(request.Email)}";

            // Send email
            try
            {
                await _emailService.SendPasswordResetEmailAsync(request.Email, resetLink);
            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Failed to send password reset email to {Email}", request.Email);
                return StatusCode(500, new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to send password reset email. Please try again later."
                });
            }

            return Ok(new ServiceResponse
            {
                Success = true,
                Message = "If your email exists in our system, you will receive a password reset link."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForgotPassword");
            return StatusCode(500, new ServiceResponse
            {
                Success = false,
                Message = "An error occurred while processing your request."
            });
        }
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ServiceResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new ServiceResponse
                {
                    Success = false,
                    Message = "Invalid password reset request."
                });
            }

            // Reset password using ASP.NET Identity
            var resetResult = await _userManagement.ResetPassword(user, request.Token, request.NewPassword);
            if (!resetResult)
            {
                return BadRequest(new ServiceResponse { Success = false, Message = "Invalid or expired password reset token." });
            }

            return Ok(new ServiceResponse
            {
                Success = true,
                Message = "Password has been reset successfully. You can now login with your new password."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ResetPassword");
            return StatusCode(500, new ServiceResponse
            {
                Success = false,
                Message = "An error occurred while resetting your password."
            });
        }
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<ServiceResponse>> VerifyEmail([FromQuery] string email, [FromQuery] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new ServiceResponse
                {
                    Success = false,
                    Message = "Invalid verification request."
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new ServiceResponse
                {
                    Success = false,
                    Message = "Invalid verification request."
                });
            }

            // Check if already verified
            var isAlreadyVerified = await _userManagement.IsEmailConfirmed(user);
            if (isAlreadyVerified)
            {
                return Ok(new ServiceResponse
                {
                    Success = true,
                    Message = "Email is already verified. You can now login."
                });
            }

            // Confirm email
            var confirmResult = await _userManagement.ConfirmEmail(user, token);
            if (!confirmResult)
            {
                return BadRequest(new ServiceResponse { Success = false, Message = "Invalid or expired verification token. Please request a new verification email." });
            }

            return Ok(new ServiceResponse
            {
                Success = true,
                Message = "Email verified successfully! You can now login to your account."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in VerifyEmail");
            return StatusCode(500, new ServiceResponse
            {
                Success = false,
                Message = "An error occurred while verifying your email."
            });
        }
    }

    [HttpPost("resend-verification")]
    public async Task<ActionResult<ServiceResponse>> ResendVerificationEmail([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that user doesn't exist
                return Ok(new ServiceResponse
                {
                    Success = true,
                    Message = "If your email exists in our system, you will receive a verification link."
                });
            }

            // Check if already verified
            var isAlreadyVerified = await _userManager.IsEmailConfirmedAsync(user);
            if (isAlreadyVerified)
            {
                return BadRequest(new ServiceResponse
                {
                    Success = false,
                    Message = "Email is already verified. You can login to your account."
                });
            }

            // Generate new token
            var emailToken = await _userManagement.GenerateEmailConfirmationToken(user);

            // Create verification link
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
            var verificationLink = $"{frontendUrl}/verify-email?token={Uri.EscapeDataString(emailToken)}&email={Uri.EscapeDataString(request.Email)}";

            // Send email
            try
            {
                await _emailService.SendEmailVerificationAsync(request.Email, verificationLink);
            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Failed to resend verification email to {Email}", request.Email);
                return StatusCode(500, new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to send verification email. Please try again later."
                });
            }

            return Ok(new ServiceResponse
            {
                Success = true,
                Message = "Verification email sent! Please check your inbox."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ResendVerificationEmail");
            return StatusCode(500, new ServiceResponse
            {
                Success = false,
                Message = "An error occurred while sending verification email."
            });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Service = "Identity API", Timestamp = DateTime.UtcNow });
    }
}
