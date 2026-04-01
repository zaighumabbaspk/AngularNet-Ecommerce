using eCommerce.Domain.Entities;

namespace eCommerce.Domain.Interface
{
    public interface IWishlistRepository : IGeneric<Wishlist>
    {
        Task<IEnumerable<Wishlist>> GetUserWishlistAsync(string userId);
        Task<Wishlist?> GetWishlistItemAsync(string userId, Guid productId);
        Task<bool> IsInWishlistAsync(string userId, Guid productId);
    }
}