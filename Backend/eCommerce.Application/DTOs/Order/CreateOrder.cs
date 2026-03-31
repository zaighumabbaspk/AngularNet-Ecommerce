namespace eCommerce.Application.DTOs.Order
{
    public class CreateOrder : OrderBase
    {
        public string? StripeSessionId { get; set; }
        public List<CreateOrderItem> OrderItems { get; set; } = new();
    }

    public class CreateOrderItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}