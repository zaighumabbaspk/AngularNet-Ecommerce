using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class RecentlyViewedRepository : GenericRepository<RecentlyViewed>, IRecentlyViewedRepository
    {
        private readonly AppDbContext _context;

        public RecentlyViewedRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RecentlyViewed>> GetUserRecentlyViewedAsync(string userId, int limit = 10)
        {
            return await _context.RecentlyViewed
                .Where(rv => rv.UserId == userId)
                .OrderByDescending(rv => rv.ViewedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<RecentlyViewed?> GetRecentlyViewedItemAsync(string userId, Guid productId)
        {
            return await _context.RecentlyViewed
                .FirstOrDefaultAsync(rv => rv.UserId == userId && rv.ProductId == productId);
        }

        public async Task<int> ClearUserRecentlyViewedAsync(string userId)
        {
            var userItems = await _context.RecentlyViewed
                .Where(rv => rv.UserId == userId)
                .ToListAsync();

            _context.RecentlyViewed.RemoveRange(userItems);
            return await _context.SaveChangesAsync();
        }
    }
}