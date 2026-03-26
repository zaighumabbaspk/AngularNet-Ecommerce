using System.ComponentModel.DataAnnotations;

namespace eCommerce.Domain.Entities
{
    public class OrderStatusHistory
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [Required]
        public DateTime ChangedAt { get; set; }
        
        [Required]
        public string ChangedBy { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
    }


}