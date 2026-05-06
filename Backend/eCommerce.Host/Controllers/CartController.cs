using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Cart;

namespace eCommerce.Host.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsync(GetUserId());

            if (cart == null)
            {
                return Ok(new GetCart
                {
                    Id = Guid.Empty,
                    UserId = GetUserId(),
                    CartItems = new List<GetCartItem>(),
                    Total = 0,
                    TotalItems = 0
                });
            }

            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartService.AddToCartAsync(GetUserId(), request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("items")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartService.UpdateCartItemAsync(GetUserId(), request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("items/{cartItemId:guid}")]
        public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
        {
            var result = await _cartService.RemoveCartItemAsync(GetUserId(), cartItemId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCartAsync(GetUserId());

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}