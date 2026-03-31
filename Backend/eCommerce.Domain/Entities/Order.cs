using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eCommerce.Domain.Entities.Identity;

namespace eCommerce.Domain.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
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
        
        // Address Information
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Required]
        public string BillingAddress { get; set; } = string.Empty;
        
        // Payment Information
        public string? StripePaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        
        // Navigation Properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
}