using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Cart;


namespace eCommerce.Application.Services.Interfaces
{
    public interface ICartService
    {
        Task<GetCart?> GetCartAsync(string userId);
        Task<ServiceResponse> AddToCartAsync(string userId, AddToCartRequest request);
        Task<ServiceResponse> UpdateCartItemAsync(string userId, UpdateCartItemRequest request);
        Task<ServiceResponse> RemoveCartItemAsync(string userId, Guid cartItemId);
        Task<ServiceResponse> ClearCartAsync(string userId);

    }

}
