using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce.Domain.Entities
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }

        public string? UserId { get; set; } 

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        [NotMapped]
        public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
