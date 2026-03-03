namespace eCommerce.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
