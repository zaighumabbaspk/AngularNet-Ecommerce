using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class SearchAnalyticsRepository : GenericRepository<SearchAnalytics>, ISearchAnalyticsRepository
    {
        private readonly AppDbContext _context;

        public SearchAnalyticsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SearchAnalytics>> GetPopularSearchTermsAsync(int limit = 10)
        {
            return await _context.SearchAnalytics
                .OrderByDescending(sa => sa.SearchCount)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<SearchAnalytics?> GetSearchAnalyticsByTermAsync(string searchTerm)
        {
            return await _context.SearchAnalytics
                .FirstOrDefaultAsync(sa => sa.SearchTerm.ToLower() == searchTerm.ToLower());
        }
    }
}