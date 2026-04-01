using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Order;
using eCommerce.Domain.Entities;

namespace eCommerce.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResponse<GetOrder>> GetOrderByIdAsync(Guid orderId, string userId);
        Task<ServiceResponse<IEnumerable<OrderSummary>>> GetUserOrdersAsync(string userId);
        Task<ServiceResponse<IEnumerable<GetOrder>>> GetUserOrdersWithDetailsAsync(string userId);
        Task<ServiceResponse<GetOrder>> CreateOrderFromCartAsync(string userId, CreateOrder createOrder);
        Task<ServiceResponse<GetOrder>> CreateOrderFromStripeSessionAsync(string stripeSessionId);
        Task<ServiceResponse<GetOrder>> CreateOrderAsync(CreateOrder createOrder);
        Task<ServiceResponse> UpdateOrderStatusAsync(UpdateOrderStatus updateStatus, string changedBy);
        Task<ServiceResponse<IEnumerable<GetOrder>>> GetOrdersByStatusAsync(OrderStatus status);
        Task<ServiceResponse<IEnumerable<GetOrder>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ServiceResponse<IEnumerable<GetOrder>>> GetAllOrdersAsync();
        Task<ServiceResponse<IEnumerable<GetOrderStatusHistory>>> GetOrderStatusHistoryAsync(Guid orderId, string userId);
    }
}