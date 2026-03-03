using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Identity
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
    }
}
