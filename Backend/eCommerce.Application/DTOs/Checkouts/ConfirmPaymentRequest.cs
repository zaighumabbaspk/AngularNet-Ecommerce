using System.ComponentModel.DataAnnotations;

namespace eCommerce.Application.DTOs.Checkouts
{
    public class ConfirmPaymentRequest
    {
        [Required]
        public string PaymentIntentId { get; set; } = string.Empty;
        
        public string PaymentMethodId { get; set; } = string.Empty;

        // Customer Information
        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        // Shipping Address
        [Required]
        [StringLength(200)]
        public string ShippingAddressLine1 { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string ShippingAddressLine2 { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string ShippingCity { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string ShippingState { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string ShippingZipCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string ShippingCountry { get; set; } = string.Empty;

        // Billing Address
        public bool BillingSameAsShipping { get; set; } = true;
        
        [StringLength(200)]
        public string BillingAddressLine1 { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string BillingAddressLine2 { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string BillingCity { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string BillingState { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string BillingZipCode { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string BillingCountry { get; set; } = string.Empty;

        // Shipping Options
        [Required]
        public string ShippingMethod { get; set; } = "standard";
        
        // Additional Information
        [StringLength(500)]
        public string SpecialInstructions { get; set; } = string.Empty;
        
        public bool IsGift { get; set; } = false;
        
        [StringLength(300)]
        public string GiftMessage { get; set; } = string.Empty;

        // Legacy properties for backward compatibility
        public string ShippingAddress => $"{ShippingAddressLine1}, {ShippingAddressLine2}, {ShippingCity}, {ShippingState} {ShippingZipCode}, {ShippingCountry}".Replace(", ,", ",").Trim(',', ' ');
        
        public string BillingAddress => BillingSameAsShipping ? ShippingAddress : 
            $"{BillingAddressLine1}, {BillingAddressLine2}, {BillingCity}, {BillingState} {BillingZipCode}, {BillingCountry}".Replace(", ,", ",").Trim(',', ' ');
    }
}