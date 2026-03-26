using eCommerce.Domain.Entities;

namespace eCommerce.Application.DTOs.Order
{
    public class OrderSummary
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
    }
}