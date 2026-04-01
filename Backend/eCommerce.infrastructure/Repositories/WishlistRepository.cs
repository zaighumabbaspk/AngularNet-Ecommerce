using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Wishlist>> GetUserWishlistAsync(string userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<Wishlist?> GetWishlistItemAsync(string userId, Guid productId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<bool> IsInWishlistAsync(string userId, Guid productId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
}