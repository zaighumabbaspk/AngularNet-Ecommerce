using eCommerce.Domain.Entities;

namespace eCommerce.Application.DTOs.Order
{
    public class UpdateOrderStatus
    {
        public Guid OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}