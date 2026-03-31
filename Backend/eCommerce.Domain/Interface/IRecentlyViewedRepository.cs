using eCommerce.Domain.Entities;

namespace eCommerce.Domain.Interface
{
    public interface IRecentlyViewedRepository : IGeneric<RecentlyViewed>
    {
        Task<IEnumerable<RecentlyViewed>> GetUserRecentlyViewedAsync(string userId, int limit = 10);
        Task<RecentlyViewed?> GetRecentlyViewedItemAsync(string userId, Guid productId);
        Task<int> ClearUserRecentlyViewedAsync(string userId);
    }
}