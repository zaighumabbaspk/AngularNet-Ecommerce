namespace eCommerce.Application.DTOs.Order
{
    public class CreateOrderResponse
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        
        // For guest orders
        public string? GuestOrderToken { get; set; }
        public bool IsGuestOrder { get; set; }
        
        // Payment information
        public string? StripePaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        
        // Tracking information
        public string TrackingUrl { get; set; } = string.Empty;
        
        public string Message { get; set; } = "Order created successfully";
    }
}