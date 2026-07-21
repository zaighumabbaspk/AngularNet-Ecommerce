using System.ComponentModel.DataAnnotations;

namespace eCommerce.Identity.API.Models;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
