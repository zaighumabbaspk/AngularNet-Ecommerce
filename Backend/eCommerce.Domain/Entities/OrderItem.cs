using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        
        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        
        // Snapshot of product details at time of order
        [Required]
        public string ProductName { get; set; } = string.Empty;
        
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
    }
}