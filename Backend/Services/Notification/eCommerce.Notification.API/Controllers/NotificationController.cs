using eCommerce.Notification.API.Models;
using eCommerce.Notification.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Notification.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(IEmailService emailService, ILogger<NotificationController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        if (string.IsNullOrEmpty(request.To) || string.IsNullOrEmpty(request.Subject))
        {
            return BadRequest(new { success = false, message = "To and Subject are required" });
        }

        var result = await _emailService.SendEmailAsync(request);

        if (result)
        {
            return Ok(new { success = true, message = "Email sent successfully" });
        }

        return StatusCode(500, new { success = false, message = "Failed to send email" });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Notification API", timestamp = DateTime.UtcNow });
    }
}
