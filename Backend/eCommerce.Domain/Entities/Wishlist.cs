using eCommerce.Domain.Entities.Identity;

namespace eCommerce.Domain.Entities
{
    public class Wishlist
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public DateTime CreatedAt { get; set; }

        public AppUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}