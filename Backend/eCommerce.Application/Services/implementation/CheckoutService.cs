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
        private readonly IOrderService _orderService;

        public CheckoutService(ICartRepository cartRepository, IOrderRepository orderRepository, IOrderService orderService, IOptions<StripeSettings> stripeSettings)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _orderService = orderService;
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
                        UnitAmount = (long)(item.Product.Price * 100), // Convert to smallest currency unit
                        Currency = "pkr", // Use PKR for Pakistani customers
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

        // Guest Checkout Methods
        public async Task<ServiceResponse<GuestPaymentIntentResponse>> CreateGuestPaymentIntentAsync(GuestCheckoutRequest request)
        {
            try
            {
                var guestToken = Guid.NewGuid().ToString("N")[..10].ToUpper();
                var totalAmount = request.TotalAmount;
                var tax = totalAmount * 0.17m; // 17% GST
                var shipping = request.ShippingMethod == "express" ? 500 : 250; // PKR
                var finalAmount = totalAmount + tax + shipping;

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(finalAmount * 100), // Convert to paisa for PKR
                    Currency = "pkr",
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "guest_email", request.GuestEmail },
                        { "guest_token", guestToken },
                        { "is_guest_order", "true" },
                        { "customer_name", request.FullName },
                        { "phone", request.Phone }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                var response = new GuestPaymentIntentResponse
                {
                    ClientSecret = paymentIntent.ClientSecret,
                    PaymentIntentId = paymentIntent.Id,
                    Amount = finalAmount,
                    Currency = "pkr",
                    GuestOrderToken = guestToken
                };

                return new ServiceResponse<GuestPaymentIntentResponse>(true, "Payment intent created successfully", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GuestPaymentIntentResponse>(false, $"Error creating payment intent: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<CreateOrderResponse>> ConfirmGuestPaymentAsync(ConfirmGuestPaymentRequest request)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(request.PaymentIntentId);

                if (paymentIntent.Status != "succeeded")
                {
                    return new ServiceResponse<CreateOrderResponse>(false, "Payment not completed");
                }

                // Extract guest info from payment intent metadata
                var guestEmail = paymentIntent.Metadata["guest_email"];
                var guestToken = paymentIntent.Metadata["guest_token"];

                if (guestEmail != request.GuestEmail || guestToken != request.GuestOrderToken)
                {
                    return new ServiceResponse<CreateOrderResponse>(false, "Invalid payment verification");
                }

                // Create guest checkout request from payment intent
                var guestCheckoutRequest = new GuestCheckoutRequest
                {
                    GuestEmail = guestEmail,
                    FirstName = paymentIntent.Metadata.GetValueOrDefault("customer_name", "").Split(' ').FirstOrDefault() ?? "",
                    LastName = paymentIntent.Metadata.GetValueOrDefault("customer_name", "").Split(' ').Skip(1).FirstOrDefault() ?? "",
                    Phone = paymentIntent.Metadata.GetValueOrDefault("phone", ""),
                    PaymentMethodId = paymentIntent.Id,
                    CartItems = new List<CartItemDto>(), // This should be stored somewhere or passed differently
                    ShippingAddress = new AddressDto(), // This should be stored somewhere or passed differently
                    CreateAccountAfterPurchase = false
                };

                // Create the order using OrderService
                var orderResult = await _orderService.CreateGuestOrderAsync(guestCheckoutRequest);

                if (!orderResult.Success)
                {
                    return new ServiceResponse<CreateOrderResponse>(false, orderResult.Message);
                }

                return new ServiceResponse<CreateOrderResponse>(true, "Guest order confirmed successfully", orderResult.Data);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CreateOrderResponse>(false, $"Error confirming guest payment: {ex.Message}");
            }
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

                // Calculate totals based on shipping method and location
                var subtotal = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);
                var shippingCost = CalculateShippingCost(request.ShippingMethod, request.ShippingCountry);
                var tax = CalculateTax(subtotal, request.ShippingState, request.ShippingCountry);
                var total = subtotal + tax + shippingCost;
                var amountInCents = (long)(total * 100);

                // Determine currency based on shipping country
                var currency = request.ShippingCountry == "PK" ? "pkr" : "usd";
                
                // For PKR, Stripe expects amount in paisa (1 PKR = 100 paisa)
                // For USD, Stripe expects amount in cents (1 USD = 100 cents)
                var stripeAmount = currency == "pkr" ? (long)(total * 100) : (long)(total * 100);

                // Create Payment Intent with comprehensive metadata
                var options = new PaymentIntentCreateOptions
                {
                    Amount = stripeAmount,
                    Currency = currency,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "userId", userId },
                        { "customerEmail", request.CustomerEmail },
                        { "customerName", request.CustomerName },
                        { "phoneNumber", request.PhoneNumber },
                        { "companyName", request.CompanyName ?? "" },
                        { "shippingAddressLine1", request.ShippingAddressLine1 },
                        { "shippingAddressLine2", request.ShippingAddressLine2 ?? "" },
                        { "shippingCity", request.ShippingCity },
                        { "shippingState", request.ShippingState },
                        { "shippingZipCode", request.ShippingZipCode },
                        { "shippingCountry", request.ShippingCountry },
                        { "billingSameAsShipping", request.BillingSameAsShipping.ToString() },
                        { "billingAddressLine1", request.BillingAddressLine1 ?? "" },
                        { "billingAddressLine2", request.BillingAddressLine2 ?? "" },
                        { "billingCity", request.BillingCity ?? "" },
                        { "billingState", request.BillingState ?? "" },
                        { "billingZipCode", request.BillingZipCode ?? "" },
                        { "billingCountry", request.BillingCountry ?? "" },
                        { "shippingMethod", request.ShippingMethod },
                        { "specialInstructions", request.SpecialInstructions ?? "" },
                        { "isGift", request.IsGift.ToString() },
                        { "giftMessage", request.GiftMessage ?? "" },
                        { "newsletterSubscription", request.NewsletterSubscription.ToString() },
                        { "smsUpdates", request.SmsUpdates.ToString() },
                        { "subtotal", subtotal.ToString("F2") },
                        { "tax", tax.ToString("F2") },
                        { "shipping", shippingCost.ToString("F2") },
                        { "total", total.ToString("F2") }
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
                        Currency = currency
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
                Console.WriteLine($"🔍 Starting payment confirmation for payment intent: {request.PaymentIntentId}");
                
                // Retrieve the payment intent to verify it was successful
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(request.PaymentIntentId);

                Console.WriteLine($"🔍 Payment intent status: {paymentIntent.Status}");
                Console.WriteLine($"🔍 Payment intent metadata count: {paymentIntent.Metadata?.Count ?? 0}");

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

                Console.WriteLine($"🔍 Cart items count: {cart.CartItems.Count}");

                // Check if metadata exists, if not use request data
                var metadata = paymentIntent.Metadata ?? new Dictionary<string, string>();
                
                decimal subtotal, tax, shipping, total;
                
                // Try to get values from metadata first, then calculate from cart if not available
                if (metadata.ContainsKey("subtotal") && metadata.ContainsKey("tax") && metadata.ContainsKey("shipping") && metadata.ContainsKey("total"))
                {
                    Console.WriteLine("🔍 Using metadata for order totals");
                    subtotal = decimal.Parse(metadata.GetValueOrDefault("subtotal", "0"));
                    tax = decimal.Parse(metadata.GetValueOrDefault("tax", "0"));
                    shipping = decimal.Parse(metadata.GetValueOrDefault("shipping", "0"));
                    total = decimal.Parse(metadata.GetValueOrDefault("total", "0"));
                }
                else
                {
                    Console.WriteLine("🔍 Calculating order totals from cart and request");
                    // Calculate from cart and request
                    subtotal = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);
                    shipping = CalculateShippingCost(request.ShippingMethod, request.ShippingCountry);
                    tax = CalculateTax(subtotal, request.ShippingState, request.ShippingCountry);
                    total = subtotal + tax + shipping;
                }

                Console.WriteLine($"🔍 Order totals - Subtotal: {subtotal}, Tax: {tax}, Shipping: {shipping}, Total: {total}");

                // Create comprehensive order (with fallbacks for missing DB columns)
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
                    ShippingAddress = metadata.ContainsKey("shippingAddressLine1") ? 
                        BuildFullAddress(
                            metadata.GetValueOrDefault("shippingAddressLine1", ""),
                            metadata.GetValueOrDefault("shippingAddressLine2", ""),
                            metadata.GetValueOrDefault("shippingCity", ""),
                            metadata.GetValueOrDefault("shippingState", ""),
                            metadata.GetValueOrDefault("shippingZipCode", ""),
                            metadata.GetValueOrDefault("shippingCountry", "")
                        ) :
                        BuildFullAddress(
                            request.ShippingAddressLine1,
                            request.ShippingAddressLine2 ?? "",
                            request.ShippingCity,
                            request.ShippingState,
                            request.ShippingZipCode,
                            request.ShippingCountry
                        ),
                    BillingAddress = request.BillingSameAsShipping ?
                        BuildFullAddress(
                            request.ShippingAddressLine1,
                            request.ShippingAddressLine2 ?? "",
                            request.ShippingCity,
                            request.ShippingState,
                            request.ShippingZipCode,
                            request.ShippingCountry
                        ) :
                        BuildFullAddress(
                            request.BillingAddressLine1 ?? "",
                            request.BillingAddressLine2 ?? "",
                            request.BillingCity ?? "",
                            request.BillingState ?? "",
                            request.BillingZipCode ?? "",
                            request.BillingCountry ?? ""
                        ),
                    StripePaymentIntentId = request.PaymentIntentId
                };

                // Try to set enhanced properties if they exist in the database
                try
                {
                    order.CustomerEmail = metadata.GetValueOrDefault("customerEmail", request.CustomerEmail);
                    order.CustomerName = metadata.GetValueOrDefault("customerName", request.CustomerName);
                    order.PhoneNumber = metadata.GetValueOrDefault("phoneNumber", request.PhoneNumber);
                    order.CompanyName = metadata.GetValueOrDefault("companyName", request.CompanyName ?? "");
                    order.ShippingMethod = metadata.GetValueOrDefault("shippingMethod", request.ShippingMethod);
                    order.SpecialInstructions = metadata.GetValueOrDefault("specialInstructions", request.SpecialInstructions ?? "");
                    order.IsGift = bool.Parse(metadata.GetValueOrDefault("isGift", request.IsGift.ToString()));
                    order.GiftMessage = metadata.GetValueOrDefault("giftMessage", request.GiftMessage ?? "");
                    order.NewsletterSubscription = bool.Parse(metadata.GetValueOrDefault("newsletterSubscription", "false"));
                    order.SmsUpdates = bool.Parse(metadata.GetValueOrDefault("smsUpdates", "false"));
                    Console.WriteLine("🔍 Enhanced order properties set successfully");
                }
                catch (Exception enhancedPropsEx)
                {
                    Console.WriteLine($"⚠️ Could not set enhanced properties (DB migration may be needed): {enhancedPropsEx.Message}");
                    // Continue without enhanced properties - they're not critical for basic order functionality
                }

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

                Console.WriteLine($"✅ Order created successfully with ID: {order.Id}");
                Console.WriteLine($"✅ Order items count: {order.OrderItems.Count}");
                Console.WriteLine($"✅ Cart cleared for user: {userId}");

                return new ServiceResponse<GetOrder>(true, "Order created successfully", orderDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in ConfirmPaymentAsync: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return new ServiceResponse<GetOrder>(false, $"Error confirming payment: {ex.Message}");
            }
        }

        private decimal CalculateShippingCost(string shippingMethod, string country)
        {
            // Pakistan-focused shipping calculation
            if (country == "PK")
            {
                return shippingMethod switch
                {
                    "standard" => 250m, // PKR
                    "express" => 500m, // PKR
                    "same-day" => 800m, // PKR
                    "free" => 0m,
                    _ => 250m
                };
            }
            
            // International shipping (convert PKR to USD for international)
            return shippingMethod switch
            {
                "standard" => 8.99m, // USD equivalent
                "express" => 17.99m, // USD equivalent
                "overnight" => 28.99m, // USD equivalent
                "free" => 0m,
                _ => 8.99m
            };
        }

        private decimal CalculateTax(decimal subtotal, string state, string country)
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

            return subtotal * taxRate;
        }

        private string BuildFullAddress(string line1, string line2, string city, string state, string zipCode, string country)
        {
            var addressParts = new List<string>();
            
            if (!string.IsNullOrEmpty(line1)) addressParts.Add(line1);
            if (!string.IsNullOrEmpty(line2)) addressParts.Add(line2);
            if (!string.IsNullOrEmpty(city)) addressParts.Add(city);
            if (!string.IsNullOrEmpty(state)) addressParts.Add(state);
            if (!string.IsNullOrEmpty(zipCode)) addressParts.Add(zipCode);
            if (!string.IsNullOrEmpty(country)) addressParts.Add(country);

            return string.Join(", ", addressParts);
        }
    }
}