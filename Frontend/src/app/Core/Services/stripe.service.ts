import { Injectable } from '@angular/core';
import { loadStripe, Stripe, StripeElements, StripeCardElement } from '@stripe/stripe-js';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class StripeService {
  private stripePromise: Promise<Stripe | null>;
  private stripe: Stripe | null = null;
  private elements: StripeElements | null = null;
  private cardElement: StripeCardElement | null = null;

  constructor() {
    console.log('🔍 Initializing Stripe with key:', environment.stripePublishableKey);
    this.stripePromise = loadStripe(environment.stripePublishableKey);
  }

  async initializeStripe(): Promise<void> {
    try {
      console.log('🔍 Loading Stripe...');
      this.stripe = await this.stripePromise;
      
      if (!this.stripe) {
        throw new Error('Failed to load Stripe. Check your publishable key.');
      }
      
      console.log('✅ Stripe loaded successfully');
      this.elements = this.stripe.elements();
      console.log('✅ Stripe Elements created');
    } catch (error) {
      console.error('❌ Stripe initialization failed:', error);
      throw error;
    }
  }

  createCardElement(): StripeCardElement | null {
    if (!this.elements) {
      console.error('❌ Stripe Elements not initialized');
      return null;
    }
    
    try {
      console.log('🔍 Creating card element...');
      
      // Simple, reliable configuration
      this.cardElement = this.elements.create('card', {
        style: {
          base: {
            fontSize: '16px',
            color: '#424770',
            fontFamily: 'system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
            '::placeholder': {
              color: '#aab7c4',
            },
          },
          invalid: {
            color: '#9e2146',
          },
        },
        hidePostalCode: true,
      });
      
      // Add event listeners
      this.cardElement.on('ready', () => {
        console.log('✅ Card element is ready');
      });
      
      this.cardElement.on('change', (event) => {
        if (event.error) {
          console.error('❌ Card element error:', event.error.message);
        } else if (event.complete) {
          console.log('✅ Card element is complete and valid');
        }
      });
      
      console.log('✅ Card element created successfully');
      return this.cardElement;
    } catch (error) {
      console.error('❌ Failed to create card element:', error);
      return null;
    }
  }

  mountCardElement(elementId: string): void {
    if (!this.cardElement) {
      console.error('❌ Card element not created');
      return;
    }

    try {
      console.log('🔍 Mounting card element to:', elementId);
      this.cardElement.mount(`#${elementId}`);
      console.log('✅ Card element mounted successfully');
    } catch (error) {
      console.error('❌ Failed to mount card element:', error);
    }
  }

  async createPaymentMethod(billingDetails: any): Promise<any> {
    if (!this.stripe || !this.cardElement) {
      throw new Error('Stripe not initialized');
    }

    console.log('🔍 Creating payment method...');
    const { error, paymentMethod } = await this.stripe.createPaymentMethod({
      type: 'card',
      card: this.cardElement,
      billing_details: billingDetails,
    });

    if (error) {
      console.error('❌ Payment method creation failed:', error);
      throw error;
    }

    console.log('✅ Payment method created:', paymentMethod);
    return paymentMethod;
  }

  async confirmCardPayment(clientSecret: string, paymentMethodId?: string): Promise<any> {
    if (!this.stripe) {
      throw new Error('Stripe not initialized');
    }

    console.log('🔍 Confirming card payment...');
    const confirmOptions: any = {
      payment_method: paymentMethodId ? paymentMethodId : {
        card: this.cardElement,
      }
    };

    const { error, paymentIntent } = await this.stripe.confirmCardPayment(clientSecret, confirmOptions);

    if (error) {
      console.error('❌ Payment confirmation failed:', error);
      throw error;
    }

    console.log('✅ Payment confirmed:', paymentIntent);
    return paymentIntent;
  }

  destroyCardElement(): void {
    if (this.cardElement) {
      console.log('🔍 Destroying card element...');
      this.cardElement.destroy();
      this.cardElement = null;
      console.log('✅ Card element destroyed');
    }
  }

  getStripe(): Stripe | null {
    return this.stripe;
  }
}