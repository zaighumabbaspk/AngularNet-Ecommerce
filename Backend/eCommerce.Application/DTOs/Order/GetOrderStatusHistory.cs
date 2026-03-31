using eCommerce.Domain.Entities;

namespace eCommerce.Application.DTOs.Order
{
    public class GetOrderStatusHistory
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}