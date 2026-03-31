export interface CreateCheckoutSessionRequest {
  shippingAddress: string;
  billingAddress: string;
}

export interface CheckoutSessionResponse {
  sessionId: string;
  url: string;
}

export interface CreatePaymentIntentRequest {
  shippingAddress: string;
  billingAddress: string;
  customerEmail: string;
  customerName: string;
}

export interface PaymentIntentResponse {
  clientSecret: string;
  paymentIntentId: string;
  amount: number;
  currency: string;
}

export interface ConfirmPaymentRequest {
  paymentIntentId: string;
  paymentMethodId: string;
  shippingAddress: string;
  billingAddress: string;
}

export interface ServiceResponse<T> {
  success: boolean;
  message: string;
  data?: T;
}