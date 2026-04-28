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
}

export interface AddressDto {
  street: string;
  city: string;
  province: string;
  postalCode: string;
  country: string;
}

export interface CartItemDto {
  productId: string;
  productName: string;
  quantity: number;
  price: number;
  imageUrl?: string;
}

export interface GuestOrderTrackingRequest {
  email: string;
  orderNumber: string;
}

export interface GuestPaymentIntentResponse {
  clientSecret: string;
  paymentIntentId: string;
  orderToken: string;
}

export interface ConfirmGuestPaymentRequest {
  paymentIntentId: string;
  orderToken: string;
  guestEmail: string;
}

export interface GuestCheckoutStep {
  stepNumber: number;
  title: string;
  isCompleted: boolean;
  isActive: boolean;
}