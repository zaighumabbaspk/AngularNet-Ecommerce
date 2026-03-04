using eCommerce.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;

namespace eCommerce.Application.Services.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        Task<ServiceResponse> CreateUser(CreateUser user);
        Task<LoginResponse> LoginUser(LoginUser user);
        Task<LoginResponse> ReviveToken(string refreshToken);
        Task<ServiceResponse> ForgotPassword(ForgotPasswordRequest request);
        Task<ServiceResponse> ResetPassword(ResetPasswordRequest request);
        Task<ServiceResponse> VerifyEmail(string email, string token);
        Task<ServiceResponse> ResendVerificationEmail(string email);
    }
}