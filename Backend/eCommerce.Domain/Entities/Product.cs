using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? Image { get; set; }

        public int Quantity { get; set; }   

        public Category Category { get; set; }

        public Guid CategoryId { get; set; }

        // New fields for advanced search
        public string Brand { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; } = 0;
        
        public int ReviewCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
        public ICollection<RecentlyViewed> RecentlyViewedItems { get; set; } = new List<RecentlyViewed>();
    }
}
