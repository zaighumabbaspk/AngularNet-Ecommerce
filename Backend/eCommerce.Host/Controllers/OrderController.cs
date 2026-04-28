using eCommerce.Application.DTOs.Checkouts;
using eCommerce.Application.DTOs.Order;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(CreateOrder createOrder)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                createOrder.UserId = userId;
                var response = await _orderService.CreateOrderAsync(createOrder);

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("user-orders")]
        public async Task<IActionResult> GetUserOrders()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var response = await _orderService.GetUserOrdersAsync(userId);

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _orderService.GetOrderByIdAsync(orderId, userId);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _orderService.GetUserOrdersAsync(userId);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("my-orders/details")]
        public async Task<IActionResult> GetMyOrdersWithDetails()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _orderService.GetUserOrdersWithDetailsAsync(userId);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("create-from-cart")]
        public async Task<IActionResult> CreateOrderFromCart([FromBody] CreateOrder createOrder)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Log the request for debugging
                Console.WriteLine($"Creating order from cart for user: {userId}");
                Console.WriteLine($"Order details - Subtotal: {createOrder.Subtotal}, Tax: {createOrder.Tax}, Shipping: {createOrder.Shipping}, Total: {createOrder.Total}");
                Console.WriteLine($"Order items count: {createOrder.OrderItems?.Count ?? 0}");
                Console.WriteLine($"Stripe Session ID: {createOrder.StripeSessionId}");

                if (createOrder.OrderItems == null || !createOrder.OrderItems.Any())
                {
                    Console.WriteLine("❌ Order creation failed: No order items provided");
                    return BadRequest(new { message = "Order items are required" });
                }

                var result = await _orderService.CreateOrderFromCartAsync(userId, createOrder);
                
                if (result.Success)
                {
                    Console.WriteLine($"✅ Order created successfully: {result.Data?.Id}");
                    return Ok(result);
                }
                
                Console.WriteLine($"❌ Order creation failed: {result.Message}");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Order creation exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatus updateStatus)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _orderService.UpdateOrderStatusAsync(updateStatus, userId);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatusAdmin(Guid orderId, UpdateOrderStatus updateOrderStatus)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                updateOrderStatus.OrderId = orderId;
                var response = await _orderService.UpdateOrderStatusAsync(updateOrderStatus, userId);

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // Admin endpoints
        [HttpGet("admin/status/{status}")]
        [Authorize(Roles = "Admin")] // Assuming you have role-based authorization
        public async Task<IActionResult> GetOrdersByStatus(OrderStatus status)
        {
            try
            {
                var result = await _orderService.GetOrdersByStatusAsync(status);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("admin/date-range")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _orderService.GetOrdersByDateRangeAsync(startDate, endDate);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var response = await _orderService.GetAllOrdersAsync();

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{orderId}/history")]
        public async Task<IActionResult> GetOrderStatusHistory(Guid orderId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var response = await _orderService.GetOrderStatusHistoryAsync(orderId, userId);

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
        [HttpPost("guest-tracking")]
        public async Task<IActionResult> TrackGuestOrder([FromBody] GuestOrderTrackingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.GetGuestOrderAsync(request.Email, request.OrderNumber);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result);
        }

        [HttpGet("guest-orders/{email}")]
        public async Task<IActionResult> GetGuestOrdersByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required");

            var result = await _orderService.GetGuestOrdersByEmailAsync(email);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

    }
}

    // Guest Order Endpoints
