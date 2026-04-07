using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Application.Services.Interfaces.Authentication;
using eCommerce.Application.Services.Interfaces.Logging;
using eCommerce.Application.Validations.Authenticaton;
using eCommerce.Domain.Entities.Identity;
using eCommerce.Domain.Services.Interfaces.Authentication;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Validations;
using FluentValidation;
using Microsoft.Extensions.Configuration;


namespace eCommerce.Application.Services.implementation.Authentication
{


    public class AuthenticationService : IAuthenticationService
    {

        private readonly IUserManagement _userManagement;
        private readonly ITokenManagement _tokenManagement;
        private readonly IRoleManagement _roleManagement;
        private readonly IAppLogger<AuthenticationService> _logger;
        private readonly IValidator<CreateUser> _createUserValidator;
        private readonly IValidator<LoginUser> _loginUserValidator;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration; 


        public AuthenticationService(IUserManagement userManagement, ITokenManagement tokenManagement,
            IRoleManagement roleManagement , IAppLogger<AuthenticationService> logger, IValidator<CreateUser>
            createUserValiator , IValidationService validationService, IMapper mapper , IValidator<LoginUser> loginUserValidator,
            IEmailService emailService, IConfiguration configuration)
        {
            _userManagement = userManagement;
            _tokenManagement = tokenManagement;
            _roleManagement = roleManagement;
            _logger = logger;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _createUserValidator = createUserValiator;
            _loginUserValidator = loginUserValidator;
            _validationService = validationService;
            _emailService = emailService;
            _configuration = configuration;


        }
        public async Task<ServiceResponse> CreateUser(CreateUser user)
        {
            try
            {
                var _Validator = await _validationService.ValidateAsync<CreateUser>(user, _createUserValidator);
                if (!_Validator.Success) return _Validator;

                var MappedModel = _mapper.Map<AppUser>(user);
                MappedModel.UserName = user.Email;
                MappedModel.PasswordHash = user.Password;

                var result = await _userManagement.CreateUser(MappedModel, user.Password);
                if (!result)
                    return new ServiceResponse(false, "Email Already Exists or Unknown Error Occured");

                var _user = await _userManagement.GetUserByEmail(user.Email);
                // All users are assigned "User" role by default
                // Admin role must be assigned manually in the database
                bool AssignedUser = await _roleManagement.AddUserToRole(_user, "User");

                if (!AssignedUser)
                {
                    var RemovedUser = await _userManagement.RemoveUserByEmail(user.Email);
                    if (RemovedUser <= 0)
                        _logger.LogError("User Could not be assigned role",
                            new Exception($"User with email {user.Email} unable to remove as a result of role assigning issue"));

                    return new ServiceResponse(false, "Error occurred in Creating Account");
                }

                // Generate email confirmation token
                var emailToken = await _userManagement.GenerateEmailConfirmationToken(_user);
                
                // Create verification link
                var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
                var verificationLink = $"{frontendUrl}/verify-email?token={Uri.EscapeDataString(emailToken)}&email={Uri.EscapeDataString(user.Email)}";

                // Send verification email
                var emailSent = await _emailService.SendEmailVerificationAsync(user.Email, verificationLink);

                if (!emailSent)
                {
                    _logger.LogError("Failed to send verification email", new Exception($"Email to {user.Email} failed"));
                }

                return new ServiceResponse(true, "Account created successfully! Please check your email to verify your account.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating a user", ex);
                return new ServiceResponse(false, "An unexpected error occurred while creating the user");
            }
            ;
        }


        public async Task<LoginResponse> LoginUser(LoginUser user)
        {
            try
            {
                var _Validator = await _validationService.ValidateAsync(user, _loginUserValidator);
                if (!_Validator.Success)
                    return new LoginResponse(_Validator.Message);

                var MappedModel = _mapper.Map<AppUser>(user);
                MappedModel.PasswordHash = user.Password;
                bool LoginResult = await _userManagement.LoginUser(MappedModel, user.Password);
                if (!LoginResult)
                    return new LoginResponse("Invalid Email or Password");

                var _user = await _userManagement.GetUserByEmail(user.Email);
              
                if (_user == null)
                {
                    return new LoginResponse(false, "Invalid Email or Password");
                }

                // Check if email is confirmed
                var isEmailConfirmed = await _userManagement.IsEmailConfirmed(_user);
                if (!isEmailConfirmed)
                {
                    return new LoginResponse(false, "Please verify your email address before logging in. Check your inbox for the verification link.");
                }

                var Claims = await _userManagement.GetUserByClaims(_user.Email!);

                string JwtToken = _tokenManagement.GenerateToken(Claims);
                string RefreshToken = _tokenManagement.GetRefreshToken();

                int saveToken = await _tokenManagement.AddRefreshToken(_user.Id, RefreshToken);
                return saveToken > 0
                    ? new LoginResponse(true, "Login Successful", JwtToken, RefreshToken)
                    : new LoginResponse(false, "Error Occurred while saving refresh token");

            }
           
            
         catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                return new LoginResponse(false, $"An unexpected error occurred: {Ex.Message}");
            }


        }
        

        public async Task<LoginResponse> ReviveToken(string refreshToken)
        {
            bool ValidateToken = await _tokenManagement.ValidateRefreshToken(refreshToken);
            if (!ValidateToken)
                return new LoginResponse("Invalid Refresh Token");
            AppUser users = new AppUser();
            
            string UserId = await _tokenManagement.GetUserIdByRefreshToken(refreshToken);
            AppUser? user = await _userManagement.GetUserById(UserId);
            var claims = await _userManagement.GetUserByClaims(user!.Email!);
            string newJwtToken = _tokenManagement.GenerateToken(claims);
            string newRefreshToken = _tokenManagement.GetRefreshToken();
            await _tokenManagement.UpdateRefreshToken(UserId,newRefreshToken);
            return new LoginResponse(true, "Token refreshed successfully", newJwtToken, newRefreshToken);

        }

        public async Task<ServiceResponse> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(request.Email);
                if (user == null)
                {
                    // Don't reveal that the user doesn't exist for security reasons
                    return new ServiceResponse(true, "If your email exists in our system, you will receive a password reset link.");
                }

                // Generate password reset token
                var resetToken = await _userManagement.GeneratePasswordResetToken(user);
                
                // Create reset link
                var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
                var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(request.Email)}";

                // Send email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(request.Email, resetLink);

                if (!emailSent)
                {
                    _logger.LogError("Failed to send password reset email", new Exception($"Email to {request.Email} failed"));
                    return new ServiceResponse(false, "Failed to send password reset email. Please try again later.");
                }

                return new ServiceResponse(true, "If your email exists in our system, you will receive a password reset link.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ForgotPassword", ex);
                return new ServiceResponse(false, "An error occurred while processing your request.");
            }
        }

        public async Task<ServiceResponse> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(request.Email);
                if (user == null)
                {
                    return new ServiceResponse(false, "Invalid password reset request.");
                }

                // Reset password using ASP.NET Identity
                var result = await _userManagement.ResetPassword(user, request.Token, request.NewPassword);

                if (!result)
                {
                    return new ServiceResponse(false, "Invalid or expired password reset token.");
                }

                return new ServiceResponse(true, "Password has been reset successfully. You can now login with your new password.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ResetPassword", ex);
                return new ServiceResponse(false, "An error occurred while resetting your password.");
            }
        }

        public async Task<ServiceResponse> VerifyEmail(string email, string token)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    return new ServiceResponse(false, "Invalid verification request.");
                }

                // Check if already verified
                var isAlreadyVerified = await _userManagement.IsEmailConfirmed(user);
                if (isAlreadyVerified)
                {
                    return new ServiceResponse(true, "Email is already verified. You can now login.");
                }

                // Confirm email
                var result = await _userManagement.ConfirmEmail(user, token);

                if (!result)
                {
                    return new ServiceResponse(false, "Invalid or expired verification token. Please request a new verification email.");
                }

                return new ServiceResponse(true, "Email verified successfully! You can now login to your account.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in VerifyEmail", ex);
                return new ServiceResponse(false, "An error occurred while verifying your email.");
            }
        }

        public async Task<ServiceResponse> ResendVerificationEmail(string email)
        {
            try
            {
                var user = await _userManagement.GetUserByEmail(email);
                if (user == null)
                {
                    // Don't reveal that user doesn't exist
                    return new ServiceResponse(true, "If your email exists in our system, you will receive a verification link.");
                }

                // Check if already verified
                var isAlreadyVerified = await _userManagement.IsEmailConfirmed(user);
                if (isAlreadyVerified)
                {
                    return new ServiceResponse(false, "Email is already verified. You can login to your account.");
                }

                // Generate new token
                var emailToken = await _userManagement.GenerateEmailConfirmationToken(user);
                
                // Create verification link
                var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
                var verificationLink = $"{frontendUrl}/verify-email?token={Uri.EscapeDataString(emailToken)}&email={Uri.EscapeDataString(email)}";

                // Send email
                var emailSent = await _emailService.SendEmailVerificationAsync(email, verificationLink);

                if (!emailSent)
                {
                    _logger.LogError("Failed to resend verification email", new Exception($"Email to {email} failed"));
                    return new ServiceResponse(false, "Failed to send verification email. Please try again later.");
                }

                return new ServiceResponse(true, "Verification email sent! Please check your inbox.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ResendVerificationEmail", ex);
                return new ServiceResponse(false, "An error occurred while sending verification email.");
            }
        }
    }
}
