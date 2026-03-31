import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { 
  CreateCheckoutSessionRequest, 
  CheckoutSessionResponse, 
  CreatePaymentIntentRequest,
  PaymentIntentResponse,
  ConfirmPaymentRequest,
  ServiceResponse 
} from '../Models/checkout.model';
import { GetOrder } from '../Models/order.model';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  private apiUrl = `${environment.apiBaseUrl}/Checkout`;

  constructor(private http: HttpClient) {}

  // Stripe Checkout (Hosted) - Keep for backward compatibility
  createCheckoutSession(request: CreateCheckoutSessionRequest): Observable<ServiceResponse<CheckoutSessionResponse>> {
    return this.http.post<ServiceResponse<CheckoutSessionResponse>>(`${this.apiUrl}/create-session`, request);
  }

  redirectToStripe(url: string): void {
    window.location.href = url;
  }

  // Stripe.js (Embedded) - New methods
  createPaymentIntent(request: CreatePaymentIntentRequest): Observable<ServiceResponse<PaymentIntentResponse>> {
    return this.http.post<ServiceResponse<PaymentIntentResponse>>(`${this.apiUrl}/create-payment-intent`, request);
  }

  confirmPayment(request: ConfirmPaymentRequest): Observable<ServiceResponse<GetOrder>> {
    return this.http.post<ServiceResponse<GetOrder>>(`${this.apiUrl}/confirm-payment`, request);
  }
}