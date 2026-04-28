namespace eCommerce.Application.DTOs.Checkouts
{
    public class GuestPaymentIntentResponse
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "pkr";
        public string GuestOrderToken { get; set; } = string.Empty;
    }
}