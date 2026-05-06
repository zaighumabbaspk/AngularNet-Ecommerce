export interface GuestCheckoutRequest {
  guestEmail: string;
  firstName: string;
  lastName: string;
  phone: string;
  shippingAddress: AddressDto;
  billingAddress?: AddressDto;
  cartItems: CartItemDto[];
  paymentMethodId: string;
  createAccountAfterPurchase?: boolean;
  shippingMethod?: string;
  specialInstructions?: string;
  isGift?: boolean;
  giftMessage?: string;
  newsletterSubscription?: boolean;
  smsUpdates?: boolean;
}

export interface AddressDto {
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

export interface CartItemDto {
  productId: string;
  productName: string;
  quantity: number;
  productPrice: number;
  productImage?: string;
  subtotal?: number;
}

export interface GuestOrderTrackingRequest {
  email: string;
  orderNumber: string;
}

export interface GuestPaymentIntentResponse {
  clientSecret: string;
  paymentIntentId: string;
  guestOrderToken: string;
  amount: number;
  currency: string;
}

export interface ConfirmGuestPaymentRequest {
  paymentIntentId: string;
  guestOrderToken: string;
  guestEmail: string;
}

export interface GuestCheckoutStep {
  stepNumber: number;
  title: string;
  isCompleted: boolean;
  isActive: boolean;
}