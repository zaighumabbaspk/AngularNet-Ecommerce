namespace eCommerce.Application.DTOs.Checkouts
{
    public class CreatePaymentIntentRequest
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
    }
}