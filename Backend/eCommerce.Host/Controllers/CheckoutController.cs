using eCommerce.Application.DTOs.Checkouts;
using eCommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost("create-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _checkoutService.CreateCheckoutSessionAsync(request, userId);
                
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

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _checkoutService.GetWebhookSecret()
                );

                // Handle the event
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null)
                    {
                        // TODO: Create order from successful payment
                        // You can implement this by calling OrderService.CreateOrderFromStripeSessionAsync
                        // or by extracting user info from session metadata and creating the order
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest($"Webhook error: {e.Message}");
            }
        }

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Log the request for debugging
                Console.WriteLine($"Creating payment intent for user: {userId}");
                Console.WriteLine($"Shipping Country: {request.ShippingCountry}");
                Console.WriteLine($"Shipping Method: {request.ShippingMethod}");

                var result = await _checkoutService.CreatePaymentIntentAsync(request, userId);
                
                if (result.Success)
                {
                    Console.WriteLine($"Payment intent created successfully: {result.Data?.PaymentIntentId}");
                    Console.WriteLine($"Currency: {result.Data?.Currency}, Amount: {result.Data?.Amount}");
                    return Ok(result);
                }
                
                Console.WriteLine($"Payment intent creation failed: {result.Message}");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment intent creation error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Log the request for debugging
                Console.WriteLine($"Confirming payment for user: {userId}");
                Console.WriteLine($"Payment Intent ID: {request.PaymentIntentId}");
                Console.WriteLine($"Customer: {request.CustomerName} ({request.CustomerEmail})");
                Console.WriteLine($"Shipping: {request.ShippingAddressLine1}, {request.ShippingCity}, {request.ShippingState}");

                var result = await _checkoutService.ConfirmPaymentAsync(request, userId);
                
                if (result.Success)
                {
                    Console.WriteLine($"✅ Payment confirmed and order created: {result.Data?.Id}");
                    return Ok(result);
                }
                
                Console.WriteLine($"❌ Payment confirmation failed: {result.Message}");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Payment confirmation exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("payment-intent/{paymentIntentId}")]
        public async Task<IActionResult> GetPaymentIntent(string paymentIntentId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var response = await _checkoutService.GetPaymentIntentAsync(paymentIntentId);

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

        [HttpGet("calculate-tax")]
        public async Task<IActionResult> CalculateTax([FromQuery] decimal subtotal, [FromQuery] string? state, [FromQuery] string? country)
        {
            try
            {
                // Pakistan-focused tax calculation
                decimal taxRate = 0.17m; // Default 17% GST for Pakistan

                if (country == "PK")
                {
                    // Pakistan GST rates
                    taxRate = 0.17m; // 17% GST
                }
                else if (country == "US")
                {
                    taxRate = state switch
                    {
                        "CA" => 0.0975m,
                        "NY" => 0.08m,
                        "TX" => 0.0625m,
                        "FL" => 0.06m,
                        _ => 0.08m
                    };
                }
                else if (country == "CA")
                {
                    taxRate = 0.13m; // HST
                }
                else
                {
                    taxRate = 0.10m; // International
                }

                var tax = subtotal * taxRate;
                return Ok(new { success = true, data = tax });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("shipping-options")]
        public async Task<IActionResult> GetShippingOptions([FromQuery] string? zipCode, [FromQuery] string? country)
        {
            try
            {
                // Return Pakistan-focused shipping options
                var shippingOptions = new List<ShippingOption>();

                if (country == "PK")
                {
                    // Pakistani shipping options in PKR
                    shippingOptions = new List<ShippingOption>
                    {
                        new ShippingOption
                        {
                            Id = "standard",
                            Name = "Standard Delivery",
                            Description = "3-5 business days",
                            Price = 250m, // PKR
                            EstimatedDays = 5,
                            IsDefault = true
                        },
                        new ShippingOption
                        {
                            Id = "express",
                            Name = "Express Delivery",
                            Description = "1-2 business days",
                            Price = 500m, // PKR
                            EstimatedDays = 2,
                            IsDefault = false
                        },
                        new ShippingOption
                        {
                            Id = "same-day",
                            Name = "Same Day Delivery",
                            Description = "Same day (Karachi, Lahore, Islamabad only)",
                            Price = 800m, // PKR
                            EstimatedDays = 0,
                            IsDefault = false
                        },
                        new ShippingOption
                        {
                            Id = "free",
                            Name = "Free Delivery",
                            Description = "5-7 business days (orders over PKR 2000)",
                            Price = 0m, // PKR
                            EstimatedDays = 7,
                            IsDefault = false
                        }
                    };
                }
                else
                {
                    // International shipping options in USD
                    shippingOptions = new List<ShippingOption>
                    {
                        new ShippingOption
                        {
                            Id = "standard",
                            Name = "Standard Shipping",
                            Description = "5-7 business days",
                            Price = 8.99m, // USD
                            EstimatedDays = 7,
                            IsDefault = true
                        },
                        new ShippingOption
                        {
                            Id = "express",
                            Name = "Express Shipping",
                            Description = "2-3 business days",
                            Price = 17.99m, // USD
                            EstimatedDays = 3,
                            IsDefault = false
                        },
                        new ShippingOption
                        {
                            Id = "overnight",
                            Name = "Overnight Shipping",
                            Description = "1 business day",
                            Price = 28.99m, // USD
                            EstimatedDays = 1,
                            IsDefault = false
                        },
                        new ShippingOption
                        {
                            Id = "free",
                            Name = "Free Shipping",
                            Description = "7-10 business days (orders over $50)",
                            Price = 0m, // USD
                            EstimatedDays = 10,
                            IsDefault = false
                        }
                    };
                }

                return Ok(new { success = true, data = shippingOptions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // Guest Checkout Endpoints
        [HttpPost("guest-payment-intent")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateGuestPaymentIntent([FromBody] GuestCheckoutRequest request)
        {
            try
            {
                Console.WriteLine($"Creating guest payment intent for: {request.GuestEmail}");
                Console.WriteLine($"Guest Name: {request.FullName}");
                Console.WriteLine($"Total Amount: {request.TotalAmount}");

                var result = await _checkoutService.CreateGuestPaymentIntentAsync(request);
                
                if (result.Success)
                {
                    Console.WriteLine($"Guest payment intent created: {result.Data?.PaymentIntentId}");
                    return Ok(result);
                }
                
                Console.WriteLine($"Guest payment intent creation failed: {result.Message}");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Guest payment intent error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("confirm-guest-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmGuestPayment([FromBody] ConfirmGuestPaymentRequest request)
        {
            try
            {
                Console.WriteLine($"Confirming guest payment for: {request.GuestEmail}");
                Console.WriteLine($"Payment Intent ID: {request.PaymentIntentId}");
                Console.WriteLine($"Guest Token: {request.GuestOrderToken}");

                var result = await _checkoutService.ConfirmGuestPaymentAsync(request);
                
                if (result.Success)
                {
                    Console.WriteLine($"✅ Guest payment confirmed and order created: {result.Data?.OrderId}");
                    return Ok(result);
                }
                
                Console.WriteLine($"❌ Guest payment confirmation failed: {result.Message}");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Guest payment confirmation exception: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}