
using Stripe.Checkout;

namespace eCommerce.infrastructure.Services.Payments
{
    public class StripePaymentService
    {


        public async Task<Session> CreateCheckoutSessionAsync(
            List<SessionLineItemOptions> lineItems,
            string successUrl,
            string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();

            return await service.CreateAsync(options);
        }
    }
}
