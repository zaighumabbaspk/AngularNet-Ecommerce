using eCommerce.Application.DTOs.RecentlyViewed;
using eCommerce.Application.DTOs;

namespace eCommerce.Application.Services.Interfaces
{
    public interface IRecentlyViewedService
    {
        Task<ServiceResponse<string>> AddRecentlyViewedAsync(AddRecentlyViewedRequest request);
        Task<ServiceResponse<RecentlyViewedResponse>> GetRecentlyViewedAsync(string userId, int limit = 10);
        Task<ServiceResponse<string>> ClearRecentlyViewedAsync(string userId);
    }
}