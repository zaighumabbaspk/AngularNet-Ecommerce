using eCommerce.Notification.API.Models;

namespace eCommerce.Notification.API.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequest request);
}
