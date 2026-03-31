using eCommerce.Domain.Entities;

namespace eCommerce.Domain.Interface
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Order?> GetOrderByIdWithDetailsAsync(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetOrdersByUserIdWithDetailsAsync(string userId);
        Task<Order?> GetOrderByStripeSessionIdAsync(string stripeSessionId);
        Task<int> CreateOrderAsync(Order order);
        Task<int> UpdateOrderAsync(Order order);
        Task<int> UpdateOrderStatusAsync(Guid orderId, OrderStatus status, string changedBy, string? notes = null);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}