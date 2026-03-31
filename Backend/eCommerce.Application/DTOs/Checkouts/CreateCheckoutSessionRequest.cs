
namespace eCommerce.Application.DTOs.Checkouts
{
    public class CreateCheckoutSessionRequest
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
    }
}
