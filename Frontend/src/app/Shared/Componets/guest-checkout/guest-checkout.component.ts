import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

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

  private loadCart(): void {
    const cartSub = this.cartService.getCart().subscribe({
      next: (cart) => {
        this.cart = cart;
      },
      error: (error) => {
        this.notificationService.error('Failed to load cart');
      }
    });
    this.subscriptions.push(cartSub);
  }

  private async initializeStripe(): Promise<void> {
    try {
      this.stripe = await this.stripeService.getStripe();
    } catch (error) {
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

  private setupStripeCardElement(): void {
    setTimeout(() => {
      const cardContainer = document.getElementById('guest-card-element');
      if (cardContainer && this.stripe) {
        const elements = this.stripe.elements();
        this.cardElement = elements.create('card', {
          style: {
            base: {
              fontSize: '16px',
              color: '#424770',
              '::placeholder': { color: '#aab7c4' }
            }
          }
        });
        this.cardElement.mount('#guest-card-element');
      }
    }, 100);
  }

  async processGuestCheckout(): Promise<void> {
    if (!this.cart || !this.cardElement) return;

    this.isProcessing = true;

    try {
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
        createAccountAfterPurchase: this.checkoutForm.get('createAccountAfterPurchase')?.value
      };

      // Create payment intent
      const paymentIntentResponse = await this.guestCheckoutService.createGuestPaymentIntent(checkoutRequest).toPromise();
      
      if (!paymentIntentResponse?.flag) {
        throw new Error(paymentIntentResponse?.message || 'Failed to create payment intent');
      }

      // Confirm payment with Stripe
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
        throw new Error(error.message);
      }

      // Confirm payment on backend
      const confirmRequest = {
        paymentIntentId: paymentIntent.id,
        orderToken: paymentIntentResponse.data.orderToken,
        guestEmail: checkoutRequest.guestEmail
      };

      const orderResponse = await this.guestCheckoutService.confirmGuestPayment(confirmRequest).toPromise();
      
      if (orderResponse?.flag) {
        this.notificationService.success('Order placed successfully!');
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
      this.notificationService.error(error.message || 'Payment failed');
    } finally {
      this.isProcessing = false;
    }
  }

  private getShippingAddress(): AddressDto {
    return {
      street: this.checkoutForm.get('shippingStreet')?.value,
      city: this.checkoutForm.get('shippingCity')?.value,
      province: this.checkoutForm.get('shippingProvince')?.value,
      postalCode: this.checkoutForm.get('shippingPostalCode')?.value,
      country: this.checkoutForm.get('shippingCountry')?.value
    };
  }

  private getBillingAddress(): AddressDto | undefined {
    if (this.checkoutForm.get('useSameAddress')?.value) {
      return this.getShippingAddress();
    }
    
    return {
      street: this.checkoutForm.get('billingStreet')?.value,
      city: this.checkoutForm.get('billingCity')?.value,
      province: this.checkoutForm.get('billingProvince')?.value,
      postalCode: this.checkoutForm.get('billingPostalCode')?.value,
      country: this.checkoutForm.get('billingCountry')?.value
    };
  }

  private getCartItems(): CartItemDto[] {
    if (!this.cart) return [];
    
    return this.cart.cartItems.map(item => ({
      productId: item.productId,
      productName: item.productName,
      quantity: item.quantity,
      price: item.price,
      imageUrl: item.imageUrl
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