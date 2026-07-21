namespace eCommerce.Identity.API.Services;

public interface IEmailService
{
    Task<bool> SendEmailVerificationAsync(string email, string verificationLink);
    Task<bool> SendPasswordResetEmailAsync(string email, string resetLink);
}
