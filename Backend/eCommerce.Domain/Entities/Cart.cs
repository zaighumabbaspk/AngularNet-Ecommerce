
using System.ComponentModel.DataAnnotations;


namespace eCommerce.Domain.Entities
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ICollection<CartItem>? CartItems { get; set; } = new List<CartItem>();
    }
}
