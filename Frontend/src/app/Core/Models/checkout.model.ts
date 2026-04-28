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
  paymentMethodId?: string;
  
  // Customer Information
  customerEmail: string;
  customerName: string;
  phoneNumber: string;
  companyName?: string;

  // Shipping Address
  shippingAddressLine1: string;
  shippingAddressLine2?: string;
  shippingCity: string;
  shippingState: string;
  shippingZipCode: string;
  shippingCountry: string;

  // Billing Address
  billingSameAsShipping: boolean;
  billingAddressLine1?: string;
  billingAddressLine2?: string;
  billingCity?: string;
  billingState?: string;
  billingZipCode?: string;
  billingCountry?: string;

  // Shipping Options
  shippingMethod: string;

  // Additional Information
  specialInstructions?: string;
  isGift: boolean;
  giftMessage?: string;
}

export interface ServiceResponse<T> {
  success: boolean;
  message: string;
  data?: T;
}