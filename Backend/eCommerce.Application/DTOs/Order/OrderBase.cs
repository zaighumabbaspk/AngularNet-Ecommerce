using eCommerce.Domain.Entities;

namespace eCommerce.Application.DTOs.Order
{
    public class OrderBase
    {
        public string? UserId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
    }
}