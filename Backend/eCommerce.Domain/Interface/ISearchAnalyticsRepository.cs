using eCommerce.Domain.Entities;

namespace eCommerce.Domain.Interface
{
    public interface ISearchAnalyticsRepository : IGeneric<SearchAnalytics>
    {
        Task<IEnumerable<SearchAnalytics>> GetPopularSearchTermsAsync(int limit = 10);
        Task<SearchAnalytics?> GetSearchAnalyticsByTermAsync(string searchTerm);
    }
}