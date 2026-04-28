using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Checkouts;
using eCommerce.Application.DTOs.Order;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;

namespace eCommerce.Application.Services.implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IGeneric<Product> _productRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IGeneric<Product> productRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GetOrder>> GetOrderByIdAsync(Guid orderId, string userId)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdWithDetailsAsync(orderId);
                if (order == null)
                {
                    return new ServiceResponse<GetOrder>(false, "Order not found");
                }

                // Ensure user can only access their own orders
                if (order.UserId != userId)
                {
                    return new ServiceResponse<GetOrder>(false, "Access denied");
                }

                var orderDto = _mapper.Map<GetOrder>(order);
                return new ServiceResponse<GetOrder>(true, "Order retrieved successfully", orderDto);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetOrder>(false, $"Error retrieving order: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<IEnumerable<OrderSummary>>> GetUserOrdersAsync(string userId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
                var orderSummaries = orders.Select(o => new OrderSummary
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    Total = o.Total,
                    ItemCount = o.OrderItems.Count
                });

                return new ServiceResponse<IEnumerable<OrderSummary>>(true, "Orders retrieved successfully", orderSummaries);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<OrderSummary>>(false, $"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetOrder>>> GetUserOrdersWithDetailsAsync(string userId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByUserIdWithDetailsAsync(userId);
                var orderDtos = _mapper.Map<IEnumerable<GetOrder>>(orders);
                return new ServiceResponse<IEnumerable<GetOrder>>(true, "Orders retrieved successfully", orderDtos);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetOrder>>(false, $"Error retrieving orders: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<GetOrder>> CreateOrderFromCartAsync(string userId, CreateOrder createOrder)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.CartItems.Any())
                {
                    return new ServiceResponse<GetOrder>(false, "Cart is empty");
                }

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Subtotal = createOrder.Subtotal,
                    Tax = createOrder.Tax,
                    Shipping = createOrder.Shipping,
                    Total = createOrder.Total,
                    Status = OrderStatus.Pending,
                    ShippingAddress = createOrder.ShippingAddress,
                    BillingAddress = createOrder.BillingAddress,
                    StripeSessionId = createOrder.StripeSessionId
                };

                // Create order items from cart
                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new OrderItem
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
                var statusHistory = new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Status = OrderStatus.Pending,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = "System",
                    Notes = "Order created"
                };
                order.StatusHistory.Add(statusHistory);

                await _orderRepository.CreateOrderAsync(order);

                var orderDto = _mapper.Map<GetOrder>(order);
                return new ServiceResponse<GetOrder>(true, "Order created successfully", orderDto);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetOrder>(false, $"Error creating order: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<GetOrder>> CreateOrderFromStripeSessionAsync(string stripeSessionId)
        {
            try
            {
                var existingOrder = await _orderRepository.GetOrderByStripeSessionIdAsync(stripeSessionId);
                if (existingOrder != null)
                {
                    var existingOrderDto = _mapper.Map<GetOrder>(existingOrder);
                    return new ServiceResponse<GetOrder>(true, "Order already exists", existingOrderDto);
                }

                // This would typically be called from the Stripe webhook
                // You would extract order details from Stripe session metadata
                return new ServiceResponse<GetOrder>(false, "Order creation from Stripe session not implemented");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetOrder>(false, $"Error creating order from Stripe session: {ex.Message}");
            }
        }

        public async Task<ServiceResponse> UpdateOrderStatusAsync(UpdateOrderStatus updateStatus, string changedBy)
        {
            try
            {
                var result = await _orderRepository.UpdateOrderStatusAsync(
                    updateStatus.OrderId, 
                    updateStatus.Status, 
                    changedBy, 
                    updateStatus.Notes);

                if (result > 0)
                {
                    return new ServiceResponse(true, "Order status updated successfully");
                }

                return new ServiceResponse(false, "Order not found or update failed");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, $"Error updating order status: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<IEnumerable<GetOrder>>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByStatusAsync(status);
                var orderDtos = _mapper.Map<IEnumerable<GetOrder>>(orders);
                return new ServiceResponse<IEnumerable<GetOrder>>(true, "Orders retrieved successfully", orderDtos);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetOrder>>(false, $"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetOrder>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
                var orderDtos = _mapper.Map<IEnumerable<GetOrder>>(orders);
                return new ServiceResponse<IEnumerable<GetOrder>>(true, "Orders retrieved successfully", orderDtos);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetOrder>>(false, $"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<GetOrder>> CreateOrderAsync(CreateOrder createOrder)
        {
            try
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = createOrder.UserId!,
                    CreatedAt = DateTime.UtcNow,
                    Subtotal = createOrder.Subtotal,
                    Tax = createOrder.Tax,
                    Shipping = createOrder.Shipping,
                    Total = createOrder.Total,
                    Status = OrderStatus.Pending,
                    ShippingAddress = createOrder.ShippingAddress,
                    BillingAddress = createOrder.BillingAddress,
                    StripeSessionId = createOrder.StripeSessionId
                };

                // Create order items
                foreach (var item in createOrder.OrderItems)
                {
                    var product = await _productRepository.GetAsync(item.ProductId);
                    if (product == null) continue;

                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.UnitPrice * item.Quantity,
                        ProductName = product.Name,
                        ProductDescription = product.Description,
                        ProductImage = product.Image
                    };
                    order.OrderItems.Add(orderItem);
                }

                // Add initial status history
                var statusHistory = new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Status = OrderStatus.Pending,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = "System",
                    Notes = "Order created"
                };
                order.StatusHistory.Add(statusHistory);

                await _orderRepository.CreateOrderAsync(order);

                var orderDto = _mapper.Map<GetOrder>(order);
                return new ServiceResponse<GetOrder>(true, "Order created successfully", orderDto);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetOrder>(false, $"Error creating order: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetOrder>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();
                var orderDtos = _mapper.Map<IEnumerable<GetOrder>>(orders);
                return new ServiceResponse<IEnumerable<GetOrder>>(true, "Orders retrieved successfully", orderDtos);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetOrder>>(false, $"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetOrderStatusHistory>>> GetOrderStatusHistoryAsync(Guid orderId, string userId)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdWithDetailsAsync(orderId);
                if (order == null)
                {
                    return new ServiceResponse<IEnumerable<GetOrderStatusHistory>>(false, "Order not found");
                }

                // Ensure user can only access their own orders
                if (order.UserId != userId)
                {
                    return new ServiceResponse<IEnumerable<GetOrderStatusHistory>>(false, "Access denied");
                }

                var historyDtos = _mapper.Map<IEnumerable<GetOrderStatusHistory>>(order.StatusHistory);
                return new ServiceResponse<IEnumerable<GetOrderStatusHistory>>(true, "Order status history retrieved successfully", historyDtos);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetOrderStatusHistory>>(false, $"Error retrieving order status history: {ex.Message}");
            }
        }

        // Guest Checkout Methods
        public async Task<ServiceResponse<CreateOrderResponse>> CreateGuestOrderAsync(GuestCheckoutRequest request)
        {
            try
            {
                // Generate unique guest order token
                var guestToken = Guid.NewGuid().ToString("N")[..10].ToUpper();
                
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = null, // Guest order
                    GuestEmail = request.GuestEmail,
                    IsGuestOrder = true,
                    GuestOrderToken = guestToken,
                    CreatedAt = DateTime.UtcNow,
                    CustomerEmail = request.GuestEmail,
                    CustomerName = request.FullName,
                    PhoneNumber = request.Phone,
                    ShippingAddress = request.ShippingAddress.ToString(),
                    BillingAddress = request.BillingAddress?.ToString() ?? request.ShippingAddress.ToString(),
                    ShippingMethod = request.ShippingMethod,
                    SpecialInstructions = request.SpecialInstructions,
                    IsGift = request.IsGift,
                    GiftMessage = request.GiftMessage,
                    NewsletterSubscription = request.NewsletterSubscription,
                    SmsUpdates = request.SmsUpdates,
                    Status = OrderStatus.Pending,
                    Subtotal = request.TotalAmount,
                    Tax = request.TotalAmount * 0.17m, // 17% GST for Pakistan
                    Shipping = request.ShippingMethod == "express" ? 500 : 250, // PKR
                    Total = request.TotalAmount + (request.TotalAmount * 0.17m) + (request.ShippingMethod == "express" ? 500 : 250)
                };

                // Create order items
                foreach (var item in request.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.ProductPrice,
                        TotalPrice = item.Subtotal,
                        ProductName = item.ProductName,
                        ProductImage = item.ProductImage
                    };
                    order.OrderItems.Add(orderItem);
                }

                // Add initial status history
                var statusHistory = new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Status = OrderStatus.Pending,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = "Guest System",
                    Notes = "Guest order created"
                };
                order.StatusHistory.Add(statusHistory);

                await _orderRepository.CreateOrderAsync(order);

                var response = new CreateOrderResponse
                {
                    OrderId = order.Id,
                    OrderNumber = order.Id.ToString(),
                    TotalAmount = order.Total,
                    Status = order.Status.ToString(),
                    CreatedAt = order.CreatedAt,
                    CustomerEmail = order.CustomerEmail,
                    CustomerName = order.CustomerName,
                    GuestOrderToken = guestToken,
                    IsGuestOrder = true,
                    TrackingUrl = $"/guest-order-tracking?email={order.GuestEmail}&orderNumber={order.Id}"
                };

                return new ServiceResponse<CreateOrderResponse>(true, "Guest order created successfully", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CreateOrderResponse>(false, $"Error creating guest order: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<GetOrder>> GetGuestOrderAsync(string email, string orderNumber)
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();
                var order = orders.FirstOrDefault(o => 
                    o.GuestEmail == email && 
                    o.Id.ToString() == orderNumber && 
                    o.IsGuestOrder);

                if (order == null)
                {
                    return new ServiceResponse<GetOrder>(false, "Order not found");
                }

                var orderDto = _mapper.Map<GetOrder>(order);
                return new ServiceResponse<GetOrder>(true, "Order retrieved successfully", orderDto);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetOrder>(false, $"Error retrieving order: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<GetOrder>>> GetGuestOrdersByEmailAsync(string email)
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();
                var guestOrders = orders.Where(o => 
                    o.GuestEmail == email && 
                    o.IsGuestOrder)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToList();

                var orderDtos = _mapper.Map<List<GetOrder>>(guestOrders);
                return new ServiceResponse<List<GetOrder>>(true, "Orders retrieved successfully", orderDtos);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetOrder>>(false, $"Error retrieving orders: {ex.Message}");
            }
        }


    }
}