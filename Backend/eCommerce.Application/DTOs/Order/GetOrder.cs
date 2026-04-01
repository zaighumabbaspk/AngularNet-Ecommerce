using eCommerce.Domain.Entities;

namespace eCommerce.Application.DTOs.Order
{
    public class GetOrder : OrderBase
    {
        public Guid Id { get; set; }
        public new string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        public List<GetOrderItem> OrderItems { get; set; } = new();
        public List<GetOrderStatusHistory> StatusHistory { get; set; } = new();
    }
}