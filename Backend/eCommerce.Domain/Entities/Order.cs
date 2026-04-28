using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eCommerce.Domain.Entities.Identity;

namespace eCommerce.Domain.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        
        public string? UserId { get; set; }

        public string? GuestEmail { get; set; }

        public bool IsGuestOrder { get; set; }

        public string? GuestOrderToken { get; set; }

        public AppUser? User { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Shipping { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Required]
        public string BillingAddress { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? CompanyName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ShippingMethod { get; set; } = "standard";
        
        [StringLength(500)]
        public string? SpecialInstructions { get; set; }
        
        public bool IsGift { get; set; } = false;
        
        [StringLength(300)]
        public string? GiftMessage { get; set; }
        
        public bool NewsletterSubscription { get; set; } = false;
        public bool SmsUpdates { get; set; } = false;
        
        public string? StripePaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        
        // Navigation Properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
}