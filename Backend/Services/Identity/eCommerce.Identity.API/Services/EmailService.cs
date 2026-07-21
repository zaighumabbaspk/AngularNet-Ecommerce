using System.Net.Http.Json;

namespace eCommerce.Identity.API.Services;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(HttpClient httpClient, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailVerificationAsync(string email, string verificationLink)
    {
        try
        {
            var notificationServiceUrl = _configuration["NotificationServiceUrl"] ?? "http://localhost:5237";
            var emailRequest = new
            {
                to = email,
                subject = "Verify Your Email - eCommerce Platform",
                body = $@"
                    <h2>Welcome to our eCommerce Platform!</h2>
                    <p>Please verify your email address by clicking the link below:</p>
                    <p><a href='{verificationLink}' style='padding: 10px 20px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px;'>Verify Email</a></p>
                    <p>Or copy and paste this link into your browser:</p>
                    <p>{verificationLink}</p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you did not create an account, please ignore this email.</p>
                "
            };

            var response = await _httpClient.PostAsJsonAsync($"{notificationServiceUrl}/api/notification/send-email", emailRequest);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Verification email sent successfully to {Email}", email);
                return true;
            }

            _logger.LogWarning("Failed to send verification email to {Email}. Status: {StatusCode}", email, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending verification email to {Email}", email);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string email, string resetLink)
    {
        try
        {
            var notificationServiceUrl = _configuration["NotificationServiceUrl"] ?? "http://localhost:5237";
            var emailRequest = new
            {
                to = email,
                subject = "Password Reset Request - eCommerce Platform",
                body = $@"
                    <h2>Password Reset Request</h2>
                    <p>We received a request to reset your password. Click the link below to reset it:</p>
                    <p><a href='{resetLink}' style='padding: 10px 20px; background-color: #f44336; color: white; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                    <p>Or copy and paste this link into your browser:</p>
                    <p>{resetLink}</p>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you did not request a password reset, please ignore this email or contact support if you have concerns.</p>
                "
            };

            var response = await _httpClient.PostAsJsonAsync($"{notificationServiceUrl}/api/notification/send-email", emailRequest);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Password reset email sent successfully to {Email}", email);
                return true;
            }

            _logger.LogWarning("Failed to send password reset email to {Email}. Status: {StatusCode}", email, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email to {Email}", email);
            return false;
        }
    }
}
