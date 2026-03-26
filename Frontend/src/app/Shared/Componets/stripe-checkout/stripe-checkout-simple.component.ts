import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environment/environment';
import { CheckoutService } from '../../../Core/Services/checkout.service';
import { CartService } from '../../../Core/Services/cart.service';
import { OrderService } from '../../../Core/Services/order.service';
import { AuthService } from '../../../Core/Services/auth.service';

@Component({
  selector: 'app-stripe-checkout-simple',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="stripe-checkout-container">
      <div class="checkout-card">
        <div class="checkout-header">
          <h2>Complete Your Payment</h2>
          <p>Enter your payment details to complete your order</p>
        </div>

        <form [formGroup]="checkoutForm" (ngSubmit)="onSubmit()" class="checkout-form">
          <!-- Customer Information -->
          <div class="form-section">
            <h3>Customer Information</h3>
            
            <div class="form-row">
              <div class="form-group">
                <label for="email">Email Address *</label>
                <input
                  id="email"
                  type="email"
                  formControlName="email"
                  placeholder="your@email.com">
              </div>
              
              <div class="form-group">
                <label for="name">Full Name *</label>
                <input
                  id="name"
                  type="text"
                  formControlName="name"
                  placeholder="John Doe">
              </div>
            </div>
          </div>

          <!-- Payment Information -->
          <div class="form-section">
            <h3>Payment Information</h3>
            
            <!-- Stripe Card Container -->
            <div class="stripe-card-wrapper">
              <label>Card Information *</label>
              <div id="stripe-card-element" class="stripe-card-element">
                <!-- Stripe will inject the card element here -->
              </div>
              <div id="card-errors" class="card-errors" *ngIf="cardError">
                {{ cardError }}
              </div>
            </div>
            
            <div class="card-element-help">
              <p><small><strong>Test Card:</strong> 4242 4242 4242 4242, 12/25, 123</small></p>
            </div>
          </div>

          <!-- Shipping Address -->
          <div class="form-section">
            <h3>Shipping Address</h3>
            <div class="form-group">
              <label for="shippingAddress">Address *</label>
              <textarea
                id="shippingAddress"
                formControlName="shippingAddress"
                placeholder="Enter your complete shipping address"
                rows="3">
              </textarea>
            </div>
          </div>

          <!-- Error Message -->
          <div class="alert alert-danger" *ngIf="errorMessage">
            {{ errorMessage }}
          </div>

          <!-- Submit Button -->
          <div class="form-actions">
            <button
              type="button"
              class="btn btn-secondary"
              (click)="router.navigate(['/cart'])"
              [disabled]="paymentProcessing">
              Back to Cart
            </button>
            <button
              type="submit"
              class="btn btn-primary"
              [disabled]="paymentProcessing || !cardReady">
              <span *ngIf="paymentProcessing" class="spinner"></span>
              {{ paymentProcessing ? 'Processing Payment...' : 'Complete Payment' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .stripe-checkout-container {
      max-width: 800px;
      margin: 2rem auto;
      padding: 0 1rem;
    }

    .checkout-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
      overflow: hidden;
    }

    .checkout-header {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 2rem;
      text-align: center;
    }

    .checkout-header h2 {
      margin: 0 0 0.5rem 0;
      font-size: 2rem;
      font-weight: 600;
    }

    .checkout-header p {
      margin: 0;
      opacity: 0.9;
      font-size: 1.1rem;
    }

    .checkout-form {
      padding: 2rem;
    }

    .form-section {
      margin-bottom: 2rem;
    }

    .form-section h3 {
      color: #333;
      margin-bottom: 1rem;
      font-size: 1.3rem;
      font-weight: 600;
      border-bottom: 2px solid #f0f0f0;
      padding-bottom: 0.5rem;
    }

    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 1rem;
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #555;
    }

    .form-group input,
    .form-group textarea {
      width: 100%;
      padding: 0.75rem;
      border: 2px solid #e1e5e9;
      border-radius: 8px;
      font-size: 1rem;
      transition: border-color 0.3s ease;
      box-sizing: border-box;
    }

    .form-group input:focus,
    .form-group textarea:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .stripe-card-wrapper {
      margin-bottom: 1rem;
    }

    .stripe-card-wrapper label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #555;
    }

    .stripe-card-element {
      border: 2px solid #e1e5e9;
      border-radius: 8px;
      padding: 1rem;
      background: white;
      min-height: 50px;
      transition: border-color 0.3s ease;
    }

    .stripe-card-element:focus-within {
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .card-errors {
      color: #fa755a;
      font-size: 0.875rem;
      margin-top: 0.5rem;
    }

    .debug-info {
      background: #f8f9fa;
      border: 1px solid #dee2e6;
      border-radius: 4px;
      padding: 0.5rem;
      margin-bottom: 1rem;
      font-size: 0.875rem;
    }

    .card-element-help {
      margin-top: 0.5rem;
    }

    .card-element-help small {
      color: #666;
      font-style: italic;
    }

    .alert {
      padding: 1rem;
      border-radius: 8px;
      margin-bottom: 1rem;
    }

    .alert-danger {
      background-color: #f8d7da;
      border: 1px solid #f5c6cb;
      color: #721c24;
    }

    .form-actions {
      display: flex;
      gap: 1rem;
      justify-content: space-between;
      margin-top: 2rem;
      padding-top: 2rem;
      border-top: 2px solid #f0f0f0;
    }

    .btn {
      padding: 0.75rem 2rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.3s ease;
      text-decoration: none;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .btn-primary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
    }

    .btn-secondary:hover:not(:disabled) {
      background: #5a6268;
      transform: translateY(-2px);
    }

    .spinner {
      width: 16px;
      height: 16px;
      border: 2px solid transparent;
      border-top: 2px solid currentColor;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    @media (max-width: 768px) {
      .stripe-checkout-container {
        margin: 1rem auto;
        padding: 0 0.5rem;
      }
      
      .checkout-form {
        padding: 1rem;
      }
      
      .form-row {
        grid-template-columns: 1fr;
      }
      
      .form-actions {
        flex-direction: column;
      }
      
      .btn {
        width: 100%;
      }
    }
  `]
})
export class StripeCheckoutSimpleComponent implements OnInit, OnDestroy, AfterViewInit {
  checkoutForm!: FormGroup;
  errorMessage = '';
  cardError = '';
  paymentProcessing = false;
  cardReady = false;
  
  private stripe: any = null;
  private cardElement: any = null;

  constructor(
    private fb: FormBuilder,
    public router: Router,
    private checkoutService: CheckoutService,
    private cartService: CartService,
    private orderService: OrderService,
    private authService: AuthService
  ) {}

  async ngOnInit(): Promise<void> {
    console.log('🔍 StripeCheckoutSimpleComponent ngOnInit called');
    this.initializeForm();
  }

  async ngAfterViewInit(): Promise<void> {
    console.log('🔍 ngAfterViewInit called');
    await this.initializeStripe();
  }

  ngOnDestroy(): void {
    if (this.cardElement) {
      this.cardElement.destroy();
    }
  }

  private initializeForm(): void {
    // Get user info from auth service
    const userInfo = this.authService.getCurrentUser();
    
    this.checkoutForm = this.fb.group({
      email: [userInfo?.email || 'zaighum059@gmail.com', [Validators.required, Validators.email]],
      name: [userInfo?.fullName || 'zaighum abbas', [Validators.required]],
      shippingAddress: ['Lahore', [Validators.required, Validators.minLength(10)]],
    });
  }

  private async initializeStripe(): Promise<void> {
    try {
      console.log('🔍 Starting Stripe initialization...');
      
      // Ensure Stripe script is loaded
      if (!(window as any).Stripe) {
        await this.loadStripeScript();
      }
      
      console.log('🔍 Creating Stripe with key:', environment.stripePublishableKey);
      
      // Create Stripe instance
      this.stripe = (window as any).Stripe(environment.stripePublishableKey);
      
      if (!this.stripe) {
        throw new Error('Failed to create Stripe instance');
      }
      
      console.log('✅ Stripe instance created successfully');
      
      // Create elements
      const elements = this.stripe.elements();
      
      // Create card element with explicit styling
      this.cardElement = elements.create('card', {
        style: {
          base: {
            fontSize: '16px',
            color: '#424770',
            fontFamily: 'Arial, sans-serif',
            '::placeholder': {
              color: '#aab7c4',
            },
          },
          invalid: {
            color: '#9e2146',
          },
        },
      });
      
      console.log('✅ Card element created');
      
      // Wait for DOM and mount
      setTimeout(() => {
        const container = document.getElementById('stripe-card-element');
        if (container) {
          console.log('✅ Found container, mounting card element...');
          this.cardElement.mount('#stripe-card-element');
          
          // Set up event listeners
          this.cardElement.on('ready', () => {
            console.log('✅ Stripe card element is ready!');
            this.cardReady = true;
          });

          this.cardElement.on('change', (event: any) => {
            console.log('🔍 Card element change:', event);
            if (event.error) {
              this.cardError = event.error.message;
              console.error('❌ Card error:', event.error.message);
            } else {
              this.cardError = '';
            }
          });

          this.cardElement.on('focus', () => {
            console.log('🔍 Card element focused');
          });

          this.cardElement.on('blur', () => {
            console.log('🔍 Card element blurred');
          });
          
          console.log('✅ Card element mounted and event listeners added');
          
        } else {
          console.error('❌ Could not find stripe-card-element container');
          this.errorMessage = 'Payment form failed to load. Please refresh the page.';
        }
      }, 500);

    } catch (error) {
      console.error('❌ Stripe initialization failed:', error);
      this.errorMessage = 'Failed to load payment form. Please refresh the page.';
    }
  }

  private loadStripeScript(): Promise<void> {
    return new Promise((resolve, reject) => {
      if ((window as any).Stripe) {
        resolve();
        return;
      }
      
      const script = document.createElement('script');
      script.src = 'https://js.stripe.com/v3/';
      script.onload = () => {
        console.log('✅ Stripe script loaded');
        resolve();
      };
      script.onerror = () => {
        console.error('❌ Failed to load Stripe script');
        reject(new Error('Failed to load Stripe script'));
      };
      document.head.appendChild(script);
    });
  }

  async onSubmit(): Promise<void> {
    if (!this.checkoutForm.valid) {
      this.markFormGroupTouched();
      this.errorMessage = 'Please fill in all required fields.';
      return;
    }

    if (!this.cardReady) {
      this.errorMessage = 'Please wait for the payment form to load completely.';
      return;
    }

    this.paymentProcessing = true;
    this.errorMessage = '';

    try {
      // Step 1: Get cart items
      const cartResponse = await this.cartService.getCart().toPromise();
      if (!cartResponse?.cartItems?.length) {
        throw new Error('Your cart is empty. Please add items before checkout.');
      }

      const cartItems = cartResponse.cartItems;
      const totalAmount = cartResponse.total;

      // Step 2: Create Payment Intent
      const paymentIntentRequest = {
        shippingAddress: this.checkoutForm.value.shippingAddress,
        billingAddress: this.checkoutForm.value.shippingAddress,
        customerEmail: this.checkoutForm.value.email,
        customerName: this.checkoutForm.value.name
      };

      const paymentIntentResponse = await this.checkoutService.createPaymentIntent(paymentIntentRequest).toPromise();
      
      if (!paymentIntentResponse?.success) {
        throw new Error(paymentIntentResponse?.message || 'Failed to create payment intent');
      }

      const clientSecret = paymentIntentResponse.data?.clientSecret;
      if (!clientSecret) {
        throw new Error('Failed to get payment client secret');
      }

      // Step 3: Confirm payment with Stripe
      const { error, paymentIntent } = await this.stripe.confirmCardPayment(clientSecret, {
        payment_method: {
          card: this.cardElement,
          billing_details: {
            name: this.checkoutForm.value.name,
            email: this.checkoutForm.value.email,
          },
        }
      });

      if (error) {
        throw error;
      }

      if (paymentIntent.status !== 'succeeded') {
        throw new Error('Payment was not successful. Please try again.');
      }

      // Step 4: Create order in backend
      const orderRequest = {
        subtotal: totalAmount * 0.9,
        tax: totalAmount * 0.08,
        shipping: totalAmount * 0.02,
        total: totalAmount,
        shippingAddress: this.checkoutForm.value.shippingAddress,
        billingAddress: this.checkoutForm.value.shippingAddress,
        stripeSessionId: paymentIntent.id,
        orderItems: cartItems.map((item: any) => ({
          productId: item.productId,
          quantity: item.quantity,
          unitPrice: item.productPrice
        }))
      };

      const orderResponse = await this.orderService.createOrderFromCart(orderRequest).toPromise();
      
      if (!orderResponse?.success) {
        throw new Error('Payment successful but order creation failed. Please contact support with payment ID: ' + paymentIntent.id);
      }

      // Step 5: Clear cart
      try {
        await this.cartService.clearCart().toPromise();
      } catch (cartError) {
        console.warn('⚠️ Failed to clear cart, but order was successful:', cartError);
      }

      // Step 6: Show success and redirect
      const orderId = orderResponse.data?.id || 'Unknown';
      
      alert(`🎉 Payment Successful! 
Order ID: ${orderId}
Payment ID: ${paymentIntent.id}
Amount: $${(totalAmount).toFixed(2)}

Your order has been placed successfully!`);
      
      this.router.navigate(['/orders']);
      
    } catch (error: any) {
      console.error('❌ Checkout error:', error);
      this.errorMessage = error.message || 'Payment failed. Please try again.';
    } finally {
      this.paymentProcessing = false;
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.checkoutForm.controls).forEach(key => {
      const control = this.checkoutForm.get(key);
      control?.markAsTouched();
    });
  }
}