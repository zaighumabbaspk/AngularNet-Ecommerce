using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Cart;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Domain.Interface;


namespace eCommerce.Application.Services.implementation
{
    public class CartService : ICartService
    {

        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        public CartService(ICartRepository cartRepository , IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse> AddToCartAsync(string userId, AddToCartRequest request)
        {
            try
            {
                if (request.Quantity <= 0)
                {
                    return new ServiceResponse(false, "Quantity must be greater than zero.");
                }

                var result = await _cartRepository.AddOrUpdateCartItemAsync(userId, request.ProductId, request.Quantity);

                return result > 0
                    ? new ServiceResponse(true, "Item added to cart successfully.")
                    : new ServiceResponse(false, "Failed to add item to cart.");
            }
            catch (Exception ex)
            {
            
                return new ServiceResponse(false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResponse> ClearCartAsync(string userId)
        {
            var result =  await _cartRepository.ClearCartAsync(userId);

            return result > 0
                ? new ServiceResponse(true, "Cart cleared successfully.")
                : new ServiceResponse(false, "Failed to clear cart.");

        }

        public async Task<GetCart?> GetCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return null;
            }
            var result = _mapper.Map<GetCart>(cart);
            return result;
        }

        public async Task<ServiceResponse> RemoveCartItemAsync(string userId, Guid cartItemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);

            if (cartItem == null)
                return new ServiceResponse(false, "Cart item not found");

            if (cartItem.Cart.UserId != userId)
                return new ServiceResponse(false, "Unauthorized");

            await _cartRepository.RemoveCartItemAsync(cartItemId);

            return new ServiceResponse(true, "Item removed");
        }



        public async Task<ServiceResponse> UpdateCartItemAsync(
      string userId,
      UpdateCartItemRequest request)
        {
            // 1️⃣ Get cart item
            var cartItem = await _cartRepository.GetCartItemByIdAsync(request.CartItemId);

            if (cartItem == null)
                return new ServiceResponse(false, "Cart item not found");

            // 2️⃣ Ownership check (VERY IMPORTANT)
            if (cartItem.Cart.UserId != userId)
                return new ServiceResponse(false, "Unauthorized operation");

            // 3️⃣ Update quantity (or delete if <= 0 handled in repo)
            var result = await _cartRepository.UpdateCartItemQuantityAsync(
                request.CartItemId,
                request.Quantity
            );

            // 4️⃣ Return response
            return result > 0
                ? new ServiceResponse(true, "Cart item updated successfully")
                : new ServiceResponse(false, "Failed to update cart item");
        }

    }
}
