using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Checkouts;
using eCommerce.Application.DTOs.Order;

namespace eCommerce.Application.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<ServiceResponse<CheckoutSessionResponse>> CreateCheckoutSessionAsync(CreateCheckoutSessionRequest request, string userId);
        Task<ServiceResponse<PaymentIntentResponse>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, string userId);
        Task<ServiceResponse<PaymentIntentResponse>> GetPaymentIntentAsync(string paymentIntentId);
        Task<ServiceResponse<GetOrder>> ConfirmPaymentAsync(ConfirmPaymentRequest request, string userId);
        string GetWebhookSecret();
        
        // Guest Checkout Methods
        Task<ServiceResponse<GuestPaymentIntentResponse>> CreateGuestPaymentIntentAsync(GuestCheckoutRequest request);
        Task<ServiceResponse<CreateOrderResponse>> ConfirmGuestPaymentAsync(ConfirmGuestPaymentRequest request);
    }
}