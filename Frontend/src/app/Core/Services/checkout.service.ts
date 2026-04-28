import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environment/environment';
import { 
  CreateCheckoutSessionRequest, 
  CheckoutSessionResponse, 
  PaymentIntentResponse,
  ConfirmPaymentRequest,
  ServiceResponse 
} from '../Models/checkout.model';
import { GetOrder } from '../Models/order.model';
import { 
  CreatePaymentIntentRequest,
  ShippingOptionsResponse,
  ShippingOption,
  DEFAULT_SHIPPING_OPTIONS
} from '../Models/enhanced-checkout.model';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  private apiUrl = `${environment.apiBaseUrl}/Checkout`;

  constructor(private http: HttpClient) {}

  createCheckoutSession(request: CreateCheckoutSessionRequest): Observable<ServiceResponse<CheckoutSessionResponse>> {
    return this.http.post<ServiceResponse<CheckoutSessionResponse>>(`${this.apiUrl}/create-session`, request);
  }

  redirectToStripe(url: string): void {
    window.location.href = url;
  }

  createPaymentIntent(request: CreatePaymentIntentRequest): Observable<ServiceResponse<PaymentIntentResponse>> {
    return this.http.post<ServiceResponse<PaymentIntentResponse>>(`${this.apiUrl}/create-payment-intent`, request);
  }

  confirmPayment(request: ConfirmPaymentRequest): Observable<ServiceResponse<GetOrder>> {
    return this.http.post<ServiceResponse<GetOrder>>(`${this.apiUrl}/confirm-payment`, request);
  }

  // Get payment intent by ID
  getPaymentIntent(paymentIntentId: string): Observable<ServiceResponse<any>> {
    return this.http.get<ServiceResponse<any>>(`${this.apiUrl}/payment-intent/${paymentIntentId}`);
  }

  // Get available shipping options
  getShippingOptions(zipCode?: string, country?: string): Observable<ShippingOption[]> {
    // For now, return default options. In a real app, this would call the backend
    // to get dynamic shipping rates based on location
    return new Observable(observer => {
      setTimeout(() => {
        observer.next(DEFAULT_SHIPPING_OPTIONS);
        observer.complete();
      }, 500);
    });
  }

  // Calculate shipping cost based on method and location
  calculateShipping(method: string, zipCode?: string, country?: string): Observable<number> {
    // Call backend for accurate shipping calculation
    return this.http.get<{ success: boolean; data: ShippingOption[] }>(`${this.apiUrl}/shipping-options`, {
      params: {
        zipCode: zipCode || '',
        country: country || 'PK'
      }
    }).pipe(
      map(response => {
        if (response.success && response.data) {
          const option = response.data.find(opt => opt.id === method);
          return option?.price || 0;
        }
        // Fallback to default options
        const option = DEFAULT_SHIPPING_OPTIONS.find(opt => opt.id === method);
        return option?.price || 0;
      }),
      catchError(() => {
        // Fallback to default options on error
        const option = DEFAULT_SHIPPING_OPTIONS.find(opt => opt.id === method);
        return of(option?.price || 0);
      })
    );
  }

  // Validate address (mock implementation)
  validateAddress(address: any): Observable<{ isValid: boolean; suggestions?: any[] }> {
    return new Observable(observer => {
      setTimeout(() => {
        // Mock validation - in real app, use address validation service
        const isValid = address.addressLine1 && address.city && address.zipCode;
        observer.next({ isValid });
        observer.complete();
      }, 500);
    });
  }

  // Calculate tax based on location
  calculateTax(subtotal: number, state?: string, country?: string): Observable<number> {
    // Call backend for accurate tax calculation
    return this.http.get<{ success: boolean; data: number }>(`${this.apiUrl}/calculate-tax`, {
      params: {
        subtotal: subtotal.toString(),
        state: state || '',
        country: country || 'PK'
      }
    }).pipe(
      map(response => {
        if (response.success && typeof response.data === 'number') {
          return response.data;
        }
        // Fallback calculation
        return this.fallbackTaxCalculation(subtotal, state, country);
      }),
      catchError(() => {
        // Fallback calculation on error
        return of(this.fallbackTaxCalculation(subtotal, state, country));
      })
    );
  }

  private fallbackTaxCalculation(subtotal: number, state?: string, country?: string): number {
    // Pakistan-focused tax calculation
    let taxRate = 0.17; // Default 17% GST for Pakistan
    
    if (country === 'PK') {
      // Pakistan GST rates
      taxRate = 0.17; // 17% GST
    } else if (country === 'US') {
      switch (state) {
        case 'CA': taxRate = 0.0975; break;
        case 'NY': taxRate = 0.08; break;
        case 'TX': taxRate = 0.0625; break;
        case 'FL': taxRate = 0.06; break;
        default: taxRate = 0.08;
      }
    } else if (country === 'CA') {
      taxRate = 0.13; // HST
    } else {
      taxRate = 0.10; // International
    }
    
    return subtotal * taxRate;
  }
}