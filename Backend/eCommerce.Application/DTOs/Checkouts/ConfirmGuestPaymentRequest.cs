using System.ComponentModel.DataAnnotations;

namespace eCommerce.Application.DTOs.Checkouts
{
    public class ConfirmGuestPaymentRequest
    {
        [Required]
        public string PaymentIntentId { get; set; } = string.Empty;
        
        [Required]
        public string GuestOrderToken { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string GuestEmail { get; set; } = string.Empty;
    }
}