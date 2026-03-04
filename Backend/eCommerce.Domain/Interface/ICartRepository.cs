using eCommerce.Domain.Entities;

namespace eCommerce.Domain.Interface
{
    public interface ICartRepository
    {
      
        Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId);


        Task<int> AddOrUpdateCartItemAsync(string userId, Guid productId, int quantity);

        // Update cart item quantity
        Task<int> UpdateCartItemQuantityAsync(Guid cartItemId, int quantity);

        // Remove specific item from cart
        Task<int> RemoveCartItemAsync(Guid cartItemId);

        Task<Cart?> GetCartByUserIdAsync(string userId);


        // Clear all items from user's cart
        Task<int> ClearCartAsync(string userId);

        // Get specific cart item
        Task<CartItem?> GetCartItemAsync(Guid cartItemId);
    }

}
