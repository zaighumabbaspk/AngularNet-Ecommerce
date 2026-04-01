using eCommerce.Application.Configuration;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Checkouts;
using eCommerce.Application.DTOs.Order;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Domain.Interface;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace eCommerce.Application.Services.implementation
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly StripeSettings _stripeSettings;

        public CheckoutService(ICartRepository cartRepository, IOrderRepository orderRepository, IOptions<StripeSettings> stripeSettings)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<ServiceResponse<CheckoutSessionResponse>> CreateCheckoutSessionAsync(CreateCheckoutSessionRequest request, string userId)
        {
            try
            {
                // Get user's cart
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.CartItems.Any())
                {
                    return new ServiceResponse<CheckoutSessionResponse>(false, "Cart is empty");
                }

                // Create line items for Stripe
                var lineItems = cart.CartItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100), // Convert to cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                            Images = new List<string> { item.Product.Image }
                        }
                    },
                    Quantity = item.Quantity
                }).ToList();

                // Create Stripe checkout session
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = "http://localhost:4200/checkout/success?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = "http://localhost:4200/checkout/cancel",
                    Metadata = new Dictionary<string, string>
                    {
                        { "userId", userId },
                        { "shippingAddress", request.ShippingAddress },
                        { "billingAddress", request.BillingAddress }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return new ServiceResponse<CheckoutSessionResponse>(
                    true, 
                    "Checkout session created successfully",
                    new CheckoutSessionResponse
                    {
                        SessionId = session.Id,
                        Url = session.Url
                    });
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CheckoutSessionResponse>(false, $"Error creating checkout session: {ex.Message}");
            }
        }

        public string GetWebhookSecret()
        {
            return _stripeSettings.WebhookSecret;
        }

        public async Task<ServiceResponse<PaymentIntentResponse>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, string userId)
        {
            try
            {
                // Get user's cart
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.CartItems.Any())
                {
                    return new ServiceResponse<PaymentIntentResponse>(false, "Cart is empty");
                }

                // Calculate total amount in cents
                var subtotal = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);
                var tax = subtotal * 0.1m; // 10% tax
                var shipping = 10.00m; // Fixed shipping
                var total = subtotal + tax + shipping;
                var amountInCents = (long)(total * 100);

                // Create Payment Intent
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "userId", userId },
                        { "shippingAddress", request.ShippingAddress },
                        { "billingAddress", request.BillingAddress },
                        { "customerEmail", request.CustomerEmail },
                        { "customerName", request.CustomerName }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                return new ServiceResponse<PaymentIntentResponse>(
                    true,
                    "Payment intent created successfully",
                    new PaymentIntentResponse
                    {
                        ClientSecret = paymentIntent.ClientSecret,
                        PaymentIntentId = paymentIntent.Id,
                        Amount = total,
                        Currency = "usd"
                    });
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaymentIntentResponse>(false, $"Error creating payment intent: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PaymentIntentResponse>> GetPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                var response = new PaymentIntentResponse
                {
                    PaymentIntentId = paymentIntent.Id,
                    Amount = paymentIntent.Amount / 100m, // Convert from cents
                    Currency = paymentIntent.Currency,
                    Status = paymentIntent.Status
                };

                return new ServiceResponse<PaymentIntentResponse>(true, "Payment intent retrieved successfully", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaymentIntentResponse>(false, $"Error retrieving payment intent: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<GetOrder>> ConfirmPaymentAsync(ConfirmPaymentRequest request, string userId)
        {
            try
            {
                // Retrieve the payment intent to verify it was successful
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(request.PaymentIntentId);

                if (paymentIntent.Status != "succeeded")
                {
                    return new ServiceResponse<GetOrder>(false, "Payment was not successful");
                }

                // Get user's cart
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.CartItems.Any())
                {
                    return new ServiceResponse<GetOrder>(false, "Cart is empty");
                }

                // Calculate totals
                var subtotal = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);
                var tax = subtotal * 0.1m;
                var shipping = 10.00m;
                var total = subtotal + tax + shipping;

                // Create order
                var order = new eCommerce.Domain.Entities.Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Subtotal = subtotal,
                    Tax = tax,
                    Shipping = shipping,
                    Total = total,
                    Status = eCommerce.Domain.Entities.OrderStatus.Processing,
                    ShippingAddress = request.ShippingAddress,
                    BillingAddress = request.BillingAddress,
                    StripePaymentIntentId = request.PaymentIntentId
                };

                // Create order items from cart
                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new eCommerce.Domain.Entities.OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price,
                        TotalPrice = cartItem.Product.Price * cartItem.Quantity,
                        ProductName = cartItem.Product.Name,
                        ProductDescription = cartItem.Product.Description,
                        ProductImage = cartItem.Product.Image
                    };
                    order.OrderItems.Add(orderItem);
                }

                // Add initial status history
                var statusHistory = new eCommerce.Domain.Entities.OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Status = eCommerce.Domain.Entities.OrderStatus.Processing,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = "System",
                    Notes = "Payment confirmed and order created"
                };
                order.StatusHistory.Add(statusHistory);

                // Save order
                await _orderRepository.CreateOrderAsync(order);

                // Clear the cart
                await _cartRepository.ClearCartAsync(userId);

                // Map to DTO and return
                var orderDto = new GetOrder
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    CreatedAt = order.CreatedAt,
                    Status = order.Status,
                    Subtotal = order.Subtotal,
                    Tax = order.Tax,
                    Shipping = order.Shipping,
                    Total = order.Total,
                    ShippingAddress = order.ShippingAddress,
                    BillingAddress = order.BillingAddress,
                    StripePaymentIntentId = order.StripePaymentIntentId,
                    OrderItems = order.OrderItems.Select(oi => new GetOrderItem
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice,
                        ProductName = oi.ProductName,
                        ProductDescription = oi.ProductDescription,
                        ProductImage = oi.ProductImage
                    }).ToList(),
                    StatusHistory = order.StatusHistory.Select(sh => new GetOrderStatusHistory
                    {
                        Id = sh.Id,
                        Status = sh.Status,
                        ChangedAt = sh.ChangedAt,
                        ChangedBy = sh.ChangedBy,
                        Notes = sh.Notes
                    }).ToList()
                };

                return new ServiceResponse<GetOrder>(true, "Order created successfully", orderDto);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetOrder>(false, $"Error confirming payment: {ex.Message}");
            }
        }
    }
}