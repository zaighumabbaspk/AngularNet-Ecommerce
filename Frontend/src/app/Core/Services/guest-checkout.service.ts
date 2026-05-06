import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { 
  GuestCheckoutRequest, 
  GuestOrderTrackingRequest, 
  GuestPaymentIntentResponse,
  ConfirmGuestPaymentRequest 
} from '../Models/guest-checkout.model';
import { ServiceResponse } from '../Models/service-response.model';
import { GetOrder } from '../Models/order.model';

@Injectable({
  providedIn: 'root'
})
export class GuestCheckoutService {
  private baseUrl = `${environment.apiBaseUrl}`;

  constructor(private http: HttpClient) {}

  // Create payment intent for guest checkout
  createGuestPaymentIntent(request: GuestCheckoutRequest): Observable<ServiceResponse<GuestPaymentIntentResponse>> {
    return this.http.post<ServiceResponse<GuestPaymentIntentResponse>>(
      `${this.baseUrl}/checkout/guest-payment-intent`, 
      request
    );
  }

  // Confirm guest payment and create order
  confirmGuestPayment(request: ConfirmGuestPaymentRequest): Observable<ServiceResponse<GetOrder>> {
    return this.http.post<ServiceResponse<GetOrder>>(
      `${this.baseUrl}/checkout/confirm-guest-payment`, 
      request
    );
  }

  // Track guest order
  trackGuestOrder(request: GuestOrderTrackingRequest): Observable<ServiceResponse<GetOrder>> {
    return this.http.post<ServiceResponse<GetOrder>>(
      `${this.baseUrl}/order/guest-tracking`, 
      request
    );
  }

  // Get all orders for guest email
  getGuestOrdersByEmail(email: string): Observable<ServiceResponse<GetOrder[]>> {
    return this.http.get<ServiceResponse<GetOrder[]>>(
      `${this.baseUrl}/order/guest-orders/${encodeURIComponent(email)}`
    );
  }
}