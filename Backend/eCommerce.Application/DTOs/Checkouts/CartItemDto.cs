using System.ComponentModel.DataAnnotations;

namespace eCommerce.Application.DTOs.Checkouts
{
    public class CartItemDto
    {
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        public string ProductName { get; set; } = string.Empty;
        
        public string ProductImage { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal ProductPrice { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        public decimal Subtotal => ProductPrice * Quantity;
        
        public string CategoryName { get; set; } = string.Empty;
        
        public int AvailableStock { get; set; }
    }
}