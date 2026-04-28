using eCommerce.Application.DTOs.Wishlist;
using eCommerce.Application.DTOs;

namespace eCommerce.Application.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<ServiceResponse<string>> AddToWishlistAsync(AddToWishlistRequest request);
        Task<ServiceResponse<string>> RemoveFromWishlistAsync(Guid productId, string userId);
        Task<ServiceResponse<WishlistResponse>> GetUserWishlistAsync(string userId);
        Task<ServiceResponse<bool>> IsInWishlistAsync(Guid productId, string userId);
    }
}