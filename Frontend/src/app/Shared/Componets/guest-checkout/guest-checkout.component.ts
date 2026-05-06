import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription, firstValueFrom } from 'rxjs';

import { GuestCheckoutService } from '../../../Core/Services/guest-checkout.service';
import { CartService } from '../../../Core/Services/cart.service';
import { CustomNotificationService } from '../../../Core/Services/custom-notification.service';
import { StripeService } from '../../../Core/Services/stripe.service';

import { 
  GuestCheckoutRequest, 
  GuestCheckoutStep,
  AddressDto,
  CartItemDto 
} from '../../../Core/Models/guest-checkout.model';
import { GetCart } from '../../../Core/Models/cart.model';

@Component({
  selector: 'app-guest-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './guest-checkout.component.html',
  styleUrls: ['./guest-checkout.component.css']
})
export class GuestCheckoutComponent implements OnInit, OnDestroy {
  checkoutForm!: FormGroup;
  currentStep = 1;
  isProcessing = false;
  cart: GetCart | null = null;
  
  private subscriptions: Subscription[] = [];
  private stripe: any;
  private cardElement: any;

  steps: GuestCheckoutStep[] = [
    { stepNumber: 1, title: 'Contact Information', isCompleted: false, isActive: true },
    { stepNumber: 2, title: 'Shipping Address', isCompleted: false, isActive: false },
    { stepNumber: 3, title: 'Payment Information', isCompleted: false, isActive: false },
    { stepNumber: 4, title: 'Review & Confirm', isCompleted: false, isActive: false }
  ];

  pakistaniProvinces = [
    'Punjab', 'Sindh', 'Khyber Pakhtunkhwa', 'Balochistan', 
    'Gilgit-Baltistan', 'Azad Jammu and Kashmir', 'Islamabad Capital Territory'
  ];

  constructor(
    private fb: FormBuilder,
    private guestCheckoutService: GuestCheckoutService,
    private cartService: CartService,
    private notificationService: CustomNotificationService,
    private stripeService: StripeService,
    private router: Router
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.loadCart();
    this.initializeStripe();
    
    // Ensure cart is loaded after a short delay
    setTimeout(() => {
      if (!this.cart || this.cart.cartItems.length === 0) {
        console.log('🔍 Cart not loaded, trying to reload...');
        this.cart = this.cartService.getCurrentCart();
        console.log('🔍 Reloaded cart:', this.cart);
      }
    }, 100);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private initializeForm(): void {
    this.checkoutForm = this.fb.group({
      // Step 1: Contact Information
      guestEmail: ['', [Validators.required, Validators.email]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phone: ['', [Validators.required, Validators.pattern(/^(\+92|0)?3[0-9]{9}$/)]],
      createAccountAfterPurchase: [false],

      // Step 2: Shipping Address
      shippingStreet: ['', Validators.required],
      shippingCity: ['', Validators.required],
      shippingProvince: ['', Validators.required],
      shippingPostalCode: ['', Validators.required],
      shippingCountry: ['Pakistan', Validators.required],

      // Billing Address (optional)
      useSameAddress: [true],
      billingStreet: [''],
      billingCity: [''],
      billingProvince: [''],
      billingPostalCode: [''],
      billingCountry: ['Pakistan']
    });
  }

  public loadCart(): void {
    // Get current cart state immediately for guest users
    this.cart = this.cartService.getCurrentCart();
    console.log('🔍 Initial cart loaded:', this.cart);
    
    // Subscribe to cart changes
    const cartSub = this.cartService.cart$.subscribe({
      next: (cart) => {
        this.cart = cart;
        console.log('🔍 Cart updated:', this.cart);
      },
      error: (error) => {
        console.error('❌ Cart subscription error:', error);
        this.notificationService.error('Failed to load cart');
      }
    });
    this.subscriptions.push(cartSub);
  }

  private async initializeStripe(): Promise<void> {
    try {
      console.log('🔍 Initializing Stripe for guest checkout...');
      await this.stripeService.initializeStripe();
      this.stripe = this.stripeService.getStripe();
      console.log('✅ Stripe initialized successfully:', !!this.stripe);
    } catch (error) {
      console.error('❌ Failed to initialize Stripe:', error);
      this.notificationService.error('Failed to initialize payment system');
    }
  }

  nextStep(): void {
    if (this.validateCurrentStep()) {
      if (this.currentStep < 4) {
        this.steps[this.currentStep - 1].isCompleted = true;
        this.steps[this.currentStep - 1].isActive = false;
        this.currentStep++;
        this.steps[this.currentStep - 1].isActive = true;

        if (this.currentStep === 3) {
          this.setupStripeCardElement();
        }
      }
    }
  }

  previousStep(): void {
    if (this.currentStep > 1) {
      this.steps[this.currentStep - 1].isActive = false;
      this.currentStep--;
      this.steps[this.currentStep - 1].isActive = true;
      this.steps[this.currentStep - 1].isCompleted = false;
    }
  }

  private validateCurrentStep(): boolean {
    const form = this.checkoutForm;
    
    switch (this.currentStep) {
      case 1:
        return !!(form.get('guestEmail')?.valid && 
               form.get('firstName')?.valid && 
               form.get('lastName')?.valid && 
               form.get('phone')?.valid);
      case 2:
        return !!(form.get('shippingStreet')?.valid && 
               form.get('shippingCity')?.valid && 
               form.get('shippingProvince')?.valid && 
               form.get('shippingPostalCode')?.valid);
      case 3:
        return this.cardElement !== null;
      default:
        return true;
    }
  }

  public setupStripeCardElement(): void {
    console.log('🔍 Setting up Stripe card element...');
    
    // Wait a bit for DOM to be ready
    setTimeout(() => {
      const cardContainer = document.getElementById('guest-card-element');
      console.log('🔍 Card container found:', !!cardContainer);
      console.log('🔍 Stripe instance:', !!this.stripe);
      
      if (cardContainer && this.stripe) {
        try {
          // Clear any existing content
          cardContainer.innerHTML = '';
          
          const elements = this.stripe.elements();
          this.cardElement = elements.create('card', {
            style: {
              base: {
                fontSize: '16px',
                color: '#424770',
                fontFamily: 'system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
                '::placeholder': { color: '#aab7c4' },
                padding: '12px'
              },
              invalid: {
                color: '#e74c3c'
              }
            },
            hidePostalCode: true
          });
          
          this.cardElement.mount('#guest-card-element');
          
          // Add event listeners
          this.cardElement.on('ready', () => {
            console.log('✅ Stripe card element ready');
          });
          
          this.cardElement.on('change', (event: any) => {
            if (event.error) {
              console.error('❌ Card element error:', event.error);
            } else if (event.complete) {
              console.log('✅ Card element complete');
            }
          });
          
          console.log('✅ Card element mounted successfully');
        } catch (error) {
          console.error('❌ Error setting up card element:', error);
          this.notificationService.error('Failed to setup payment form');
        }
      } else {
        console.error('❌ Missing card container or Stripe instance');
        if (!cardContainer) {
          console.error('Card container not found');
        }
        if (!this.stripe) {
          console.error('Stripe not initialized');
        }
      }
    }, 200);
  }

  private debugCart(): void {
    console.log('🔍 Debug Cart Info:');
    console.log('- this.cart:', this.cart);
    console.log('- cartService.getCurrentCart():', this.cartService.getCurrentCart());
    console.log('- localStorage guest_cart:', localStorage.getItem('guest_cart'));
  }

  async processGuestCheckout(): Promise<void> {
    this.debugCart(); // Debug cart state
    
    console.log('🔍 Starting guest checkout process...');
    
    // Validation checks
    if (!this.cart || this.cart.cartItems.length === 0) {
      console.error('❌ Cart validation failed:', {
        cart: this.cart,
        hasCart: !!this.cart,
        itemCount: this.cart?.cartItems?.length || 0
      });
      this.notificationService.error('Your cart is empty');
      return;
    }
    
    if (!this.cardElement) {
      console.error('❌ Card element not available');
      this.notificationService.error('Payment form not ready. Please refresh and try again.');
      return;
    }
    
    if (!this.stripe) {
      console.error('❌ Stripe not initialized');
      this.notificationService.error('Payment system not ready. Please refresh and try again.');
      return;
    }

    this.isProcessing = true;

    try {
      console.log('🔍 Preparing checkout request...');
      
      // Prepare checkout request
      const checkoutRequest: GuestCheckoutRequest = {
        guestEmail: this.checkoutForm.get('guestEmail')?.value,
        firstName: this.checkoutForm.get('firstName')?.value,
        lastName: this.checkoutForm.get('lastName')?.value,
        phone: this.checkoutForm.get('phone')?.value,
        shippingAddress: this.getShippingAddress(),
        billingAddress: this.getBillingAddress(),
        cartItems: this.getCartItems(),
        paymentMethodId: '', // Will be set after Stripe confirmation
        createAccountAfterPurchase: this.checkoutForm.get('createAccountAfterPurchase')?.value || false,
        shippingMethod: 'standard',
        specialInstructions: '',
        isGift: false,
        giftMessage: '',
        newsletterSubscription: false,
        smsUpdates: false
      };

      console.log('🔍 Checkout request prepared:', checkoutRequest);

      // Create payment intent
      console.log('🔍 Creating payment intent...');
      const paymentIntentResponse = await firstValueFrom(this.guestCheckoutService.createGuestPaymentIntent(checkoutRequest));
      
      console.log('🔍 Payment intent response:', paymentIntentResponse);
      
      if (!paymentIntentResponse?.flag) {
        throw new Error(paymentIntentResponse?.message || 'Failed to create payment intent');
      }

      // Confirm payment with Stripe
      console.log('🔍 Confirming payment with Stripe...');
      const { error, paymentIntent } = await this.stripe.confirmCardPayment(
        paymentIntentResponse.data.clientSecret,
        {
          payment_method: {
            card: this.cardElement,
            billing_details: {
              name: `${checkoutRequest.firstName} ${checkoutRequest.lastName}`,
              email: checkoutRequest.guestEmail,
              phone: checkoutRequest.phone
            }
          }
        }
      );

      if (error) {
        console.error('❌ Stripe payment error:', error);
        throw new Error(error.message);
      }

      console.log('✅ Payment confirmed with Stripe:', paymentIntent);

      // Confirm payment on backend
      console.log('🔍 Confirming payment on backend...');
      const confirmRequest = {
        paymentIntentId: paymentIntent.id,
        guestOrderToken: paymentIntentResponse.data.guestOrderToken,
        guestEmail: checkoutRequest.guestEmail
      };

      const orderResponse = await firstValueFrom(this.guestCheckoutService.confirmGuestPayment(confirmRequest));
      
      console.log('🔍 Order confirmation response:', orderResponse);
      
      if (orderResponse?.flag) {
        console.log('✅ Order placed successfully!');
        this.notificationService.success('Order placed successfully!');
        
        // Clear guest cart after successful order
        localStorage.removeItem('guest_cart');
        
        this.router.navigate(['/guest-order-success'], { 
          queryParams: { 
            orderNumber: orderResponse.data.orderNumber,
            email: checkoutRequest.guestEmail 
          } 
        });
      } else {
        throw new Error(orderResponse?.message || 'Failed to confirm order');
      }

    } catch (error: any) {
      console.error('❌ Guest checkout error:', error);
      this.notificationService.error(error.message || 'Payment failed. Please try again.');
    } finally {
      this.isProcessing = false;
    }
  }

  private getShippingAddress(): AddressDto {
    return {
      addressLine1: this.checkoutForm.get('shippingStreet')?.value,
      addressLine2: '',
      city: this.checkoutForm.get('shippingCity')?.value,
      state: this.checkoutForm.get('shippingProvince')?.value,
      zipCode: this.checkoutForm.get('shippingPostalCode')?.value,
      country: this.checkoutForm.get('shippingCountry')?.value
    };
  }

  private getBillingAddress(): AddressDto | undefined {
    if (this.checkoutForm.get('useSameAddress')?.value) {
      return this.getShippingAddress();
    }
    
    return {
      addressLine1: this.checkoutForm.get('billingStreet')?.value,
      addressLine2: '',
      city: this.checkoutForm.get('billingCity')?.value,
      state: this.checkoutForm.get('billingProvince')?.value,
      zipCode: this.checkoutForm.get('billingPostalCode')?.value,
      country: this.checkoutForm.get('billingCountry')?.value
    };
  }

  private getCartItems(): CartItemDto[] {
    if (!this.cart) return [];
    
    return this.cart.cartItems.map(item => ({
      productId: item.productId,
      productName: item.productName,
      quantity: item.quantity,
      productPrice: item.price,
      productImage: item.imageUrl || item.productImage,
      subtotal: item.price * item.quantity
    }));
  }

  getCartTotal(): number {
    return this.cart?.cartItems.reduce((total, item) => total + (item.price * item.quantity), 0) || 0;
  }

  getShippingCost(): number {
    return 250; // Standard shipping for Pakistan
  }

  getTaxAmount(): number {
    return this.getCartTotal() * 0.17; // 17% GST
  }

  getFinalTotal(): number {
    return this.getCartTotal() + this.getShippingCost() + this.getTaxAmount();
  }
}