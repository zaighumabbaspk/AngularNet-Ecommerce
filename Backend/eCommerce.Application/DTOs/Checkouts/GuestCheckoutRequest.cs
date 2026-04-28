using System.ComponentModel.DataAnnotations;

namespace eCommerce.Application.DTOs.Checkouts
{
    public class GuestCheckoutRequest
    {
        [Required]
        [EmailAddress]
        public string GuestEmail { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        public AddressDto ShippingAddress { get; set; } = new();
        
        public AddressDto? BillingAddress { get; set; }
        
        [Required]
        public List<CartItemDto> CartItems { get; set; } = new();
        
        [Required]
        public string PaymentMethodId { get; set; } = string.Empty;
        
        public bool CreateAccountAfterPurchase { get; set; } = false;
        
        // Additional properties for enhanced checkout
        public string ShippingMethod { get; set; } = "standard";
        public string? SpecialInstructions { get; set; }
        public bool IsGift { get; set; } = false;
        public string? GiftMessage { get; set; }
        public bool NewsletterSubscription { get; set; } = false;
        public bool SmsUpdates { get; set; } = false;
        
        // Computed properties
        public string FullName => $"{FirstName} {LastName}";
        public decimal TotalAmount => CartItems?.Sum(item => item.Subtotal) ?? 0;
    }
}