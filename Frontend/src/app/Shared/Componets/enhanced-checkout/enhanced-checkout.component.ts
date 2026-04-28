import { Component, OnInit, OnDestroy, AfterViewInit, AfterViewChecked } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environment/environment';
import { CheckoutService } from '../../../Core/Services/checkout.service';
import { CartService } from '../../../Core/Services/cart.service';
import { OrderService } from '../../../Core/Services/order.service';
import { AuthService } from '../../../Core/Services/auth.service';
import { CustomNotificationService } from '../../../Core/Services/custom-notification.service';
import { 
  EnhancedCheckoutForm, 
  CreatePaymentIntentRequest, 
  ShippingOption, 
  COUNTRIES, 
  CountryState,
  DEFAULT_SHIPPING_OPTIONS
} from '../../../Core/Models/enhanced-checkout.model';

@Component({
  selector: 'app-enhanced-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './enhanced-checkout.component.html',
  styleUrls: ['./enhanced-checkout.component.css']
})
export class EnhancedCheckoutComponent implements OnInit, OnDestroy, AfterViewInit, AfterViewChecked {
  checkoutForm!: FormGroup;
  currentStep = 1;
  totalSteps = 4;
  
  // Data
  countries = COUNTRIES;
  shippingOptions: ShippingOption[] = DEFAULT_SHIPPING_OPTIONS;
  cartItems: any[] = [];
  
  // Calculations
  subtotal = 0;
  tax = 0;
  shippingCost = 0;
  total = 0;
  
  // UI State
  errorMessage = '';
  cardError = '';
  paymentProcessing = false;
  cardReady = false;
  isLoadingShipping = false;
  isCalculatingTax = false;
  
  // Stripe
  private stripe: any = null;
  private cardElement: any = null;

  constructor(
    private fb: FormBuilder,
    public router: Router,
    private checkoutService: CheckoutService,
    private cartService: CartService,
    private orderService: OrderService,
    private authService: AuthService,
    private notificationService: CustomNotificationService
  ) {}

  async ngOnInit(): Promise<void> {
    console.log('🚀 Enhanced Checkout Component Loaded!');
    this.initializeForm();
    await this.loadCartData();
    this.setupFormSubscriptions();
  }

  async ngAfterViewInit(): Promise<void> {
    await this.initializeStripe();
  }

  ngAfterViewChecked(): void {
    // Check if we're on step 4 and card element needs mounting
    if (this.currentStep === 4 && this.cardElement) {
      const container = document.getElementById('stripe-card-element');
      if (container && !container.hasChildNodes()) {
        this.mountCardElement();
      }
    }
  }

  ngOnDestroy(): void {
    if (this.cardElement) {
      this.cardElement.destroy();
    }
  }

  private initializeForm(): void {
    const userInfo = this.authService.getCurrentUser();
    
    this.checkoutForm = this.fb.group({
      // Customer Information
      customerEmail: [userInfo?.email || '', [Validators.required, Validators.email]],
      customerName: [userInfo?.fullName || '', [Validators.required, Validators.minLength(2)]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^(\+92|0)?[0-9]{10}$/)]], // Pakistani phone format
      companyName: [''],

      // Shipping Address
      shippingAddressLine1: ['', [Validators.required, Validators.minLength(5)]],
      shippingAddressLine2: [''],
      shippingCity: ['', [Validators.required, Validators.minLength(2)]],
      shippingState: ['', [Validators.required]],
      shippingZipCode: ['', [Validators.required, Validators.pattern(/^[0-9]{5}$/)]],
      shippingCountry: ['PK', [Validators.required]], // Default to Pakistan

      // Billing Address
      billingSameAsShipping: [true],
      billingAddressLine1: [''],
      billingAddressLine2: [''],
      billingCity: [''],
      billingState: [''],
      billingZipCode: [''],
      billingCountry: ['PK'], // Default to Pakistan

      // Shipping Options
      shippingMethod: ['standard', [Validators.required]],

      // Additional Information
      specialInstructions: [''],
      isGift: [false],
      giftMessage: [''],
      newsletterSubscription: [false],
      smsUpdates: [false],

      // Terms and Conditions
      acceptTerms: [false, [Validators.requiredTrue]],
      acceptPrivacy: [false, [Validators.requiredTrue]]
    });
  }

  private setupFormSubscriptions(): void {
    // Update billing address validation when checkbox changes
    this.checkoutForm.get('billingSameAsShipping')?.valueChanges.subscribe(sameAsShipping => {
      const billingFields = ['billingAddressLine1', 'billingCity', 'billingState', 'billingZipCode', 'billingCountry'];
      
      billingFields.forEach(field => {
        const control = this.checkoutForm.get(field);
        if (sameAsShipping) {
          control?.clearValidators();
        } else {
          if (field === 'billingAddressLine1' || field === 'billingCity' || field === 'billingState' || field === 'billingZipCode' || field === 'billingCountry') {
            control?.setValidators([Validators.required]);
          }
        }
        control?.updateValueAndValidity();
      });
    });

    // Recalculate shipping when method changes
    this.checkoutForm.get('shippingMethod')?.valueChanges.subscribe(method => {
      this.updateShippingCost(method);
    });

    // Recalculate tax when location changes
    this.checkoutForm.get('shippingState')?.valueChanges.subscribe(() => {
      this.calculateTax();
    });

    this.checkoutForm.get('shippingCountry')?.valueChanges.subscribe(() => {
      this.calculateTax();
    });
  }

  private async loadCartData(): Promise<void> {
    try {
      const cartResponse = await this.cartService.getCart().toPromise();
      if (cartResponse?.cartItems?.length) {
        this.cartItems = cartResponse.cartItems;
        this.subtotal = cartResponse.total || 0;
        this.updateShippingCost('standard');
        this.calculateTax();
      } else {
        this.notificationService.error('Your cart is empty');
        this.router.navigate(['/cart']);
      }
    } catch (error) {
      this.notificationService.error('Failed to load cart data');
      console.error('Cart loading error:', error);
    }
  }

  private updateShippingCost(method: string): void {
    this.isLoadingShipping = true;
    
    this.checkoutService.calculateShipping(method).subscribe({
      next: (cost) => {
        this.shippingCost = cost;
        this.calculateTotal();
        this.isLoadingShipping = false;
      },
      error: (error) => {
        console.error('Shipping calculation error:', error);
        this.isLoadingShipping = false;
      }
    });
  }

  private calculateTax(): void {
    this.isCalculatingTax = true;
    const state = this.checkoutForm.get('shippingState')?.value;
    const country = this.checkoutForm.get('shippingCountry')?.value;
    
    this.checkoutService.calculateTax(this.subtotal, state, country).subscribe({
      next: (tax) => {
        this.tax = tax;
        this.calculateTotal();
        this.isCalculatingTax = false;
      },
      error: (error) => {
        console.error('Tax calculation error:', error);
        this.isCalculatingTax = false;
      }
    });
  }

  private calculateTotal(): void {
    this.total = this.subtotal + this.tax + this.shippingCost;
  }

  private async initializeStripe(): Promise<void> {
    try {
      console.log('🔍 Starting Stripe initialization...');
      
      if (!(window as any).Stripe) {
        console.log('🔍 Loading Stripe script...');
        await this.loadStripeScript();
      }
      
      console.log('🔍 Creating Stripe instance...');
      this.stripe = (window as any).Stripe(environment.stripePublishableKey);
      
      if (!this.stripe) {
        throw new Error('Failed to create Stripe instance');
      }
      
      console.log('✅ Stripe instance created successfully');
      
      const elements = this.stripe.elements();
      
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
      
      // Don't mount immediately, wait for step 4
      this.cardElement.on('ready', () => {
        console.log('✅ Stripe card element is ready!');
        this.cardReady = true;
      });

      this.cardElement.on('change', (event: any) => {
        console.log('🔍 Card element change:', event);
        this.cardError = event.error ? event.error.message : '';
      });

    } catch (error) {
      console.error('❌ Stripe initialization failed:', error);
      this.errorMessage = 'Failed to load payment form. Please refresh the page.';
    }
  }

  // Mount card element when reaching step 4 (public method for template access)
  mountCardElement(): void {
    if (this.cardElement && this.currentStep === 4) {
      // Use a longer timeout to ensure DOM is ready
      setTimeout(() => {
        const container = document.getElementById('stripe-card-element');
        console.log('🔍 Looking for stripe-card-element container:', container);
        
        if (container) {
          // Check if already mounted
          if (container.hasChildNodes()) {
            console.log('⚠️ Card element already mounted');
            return;
          }
          
          try {
            console.log('🔍 Mounting card element...');
            this.cardElement.mount('#stripe-card-element');
            console.log('✅ Card element mounted successfully');
          } catch (error) {
            console.error('❌ Error mounting card element:', error);
            // Try to unmount and remount
            try {
              this.cardElement.unmount();
              setTimeout(() => {
                this.cardElement.mount('#stripe-card-element');
                console.log('✅ Card element remounted successfully');
              }, 100);
            } catch (remountError) {
              console.error('❌ Error remounting card element:', remountError);
              this.errorMessage = 'Failed to load payment form. Please refresh the page.';
            }
          }
        } else {
          console.error('❌ Could not find stripe-card-element container');
          this.errorMessage = 'Payment form container not found. Please refresh the page.';
        }
      }, 300); // Increased timeout
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
      script.onload = () => resolve();
      script.onerror = () => reject(new Error('Failed to load Stripe script'));
      document.head.appendChild(script);
    });
  }

  // Step Navigation
  nextStep(): void {
    if (this.validateCurrentStep()) {
      this.currentStep++;
      if (this.currentStep === 4) {
        this.mountCardElement();
      }
    }
  }

  previousStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  goToStep(step: number): void {
    if (step <= this.currentStep || this.validateStepsUpTo(step - 1)) {
      this.currentStep = step;
      if (this.currentStep === 4) {
        this.mountCardElement();
      }
    }
  }

  private validateCurrentStep(): boolean {
    switch (this.currentStep) {
      case 1: // Customer Information
        return this.validateStepFields(['customerEmail', 'customerName', 'phoneNumber']);
      case 2: // Addresses
        const shippingValid = this.validateStepFields([
          'shippingAddressLine1', 'shippingCity', 'shippingState', 'shippingZipCode', 'shippingCountry'
        ]);
        
        if (!this.checkoutForm.get('billingSameAsShipping')?.value) {
          return shippingValid && this.validateStepFields([
            'billingAddressLine1', 'billingCity', 'billingState', 'billingZipCode', 'billingCountry'
          ]);
        }
        return shippingValid;
      case 3: // Shipping & Additional Info
        return this.validateStepFields(['shippingMethod', 'acceptTerms', 'acceptPrivacy']);
      default:
        return true;
    }
  }

  private validateStepsUpTo(step: number): boolean {
    const currentStep = this.currentStep;
    let isValid = true;
    
    for (let i = 1; i <= step; i++) {
      this.currentStep = i;
      if (!this.validateCurrentStep()) {
        isValid = false;
        break;
      }
    }
    
    this.currentStep = currentStep;
    return isValid;
  }

  private validateStepFields(fields: string[]): boolean {
    let isValid = true;
    
    fields.forEach(fieldName => {
      const control = this.checkoutForm.get(fieldName);
      if (control) {
        control.markAsTouched();
        if (control.invalid) {
          isValid = false;
        }
      }
    });
    
    return isValid;
  }

  // Form Helpers
  getFieldError(fieldName: string): string {
    const control = this.checkoutForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) return `${this.getFieldLabel(fieldName)} is required`;
      if (control.errors['email']) return 'Please enter a valid email address';
      if (control.errors['minlength']) return `${this.getFieldLabel(fieldName)} is too short`;
      if (control.errors['pattern']) {
        if (fieldName === 'phoneNumber') {
          return 'Please enter a valid Pakistani phone number (e.g., +92 300 1234567 or 03001234567)';
        }
        if (fieldName === 'shippingZipCode' || fieldName === 'billingZipCode') {
          return 'Please enter a valid 5-digit postal code';
        }
        return `Please enter a valid ${this.getFieldLabel(fieldName).toLowerCase()}`;
      }
    }
    return '';
  }

  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      customerEmail: 'Email',
      customerName: 'Full Name',
      phoneNumber: 'Phone Number',
      shippingAddressLine1: 'Address',
      shippingCity: 'City',
      shippingState: 'Province',
      shippingZipCode: 'Postal Code',
      shippingCountry: 'Country',
      billingAddressLine1: 'Billing Address',
      billingCity: 'Billing City',
      billingState: 'Billing Province',
      billingZipCode: 'Billing Postal Code',
      billingCountry: 'Billing Country',
      shippingMethod: 'Shipping Method',
      acceptTerms: 'Terms & Conditions',
      acceptPrivacy: 'Privacy Policy'
    };
    return labels[fieldName] || fieldName;
  }

  getStatesForCountry(countryCode: string) {
    const country = this.countries.find(c => c.code === countryCode);
    return country?.states || [];
  }

  // Payment Processing
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
      // Create payment intent request
      const formValue = this.checkoutForm.value;
      const paymentIntentRequest: CreatePaymentIntentRequest = {
        customerEmail: formValue.customerEmail,
        customerName: formValue.customerName,
        phoneNumber: formValue.phoneNumber,
        companyName: formValue.companyName,
        shippingAddressLine1: formValue.shippingAddressLine1,
        shippingAddressLine2: formValue.shippingAddressLine2,
        shippingCity: formValue.shippingCity,
        shippingState: formValue.shippingState,
        shippingZipCode: formValue.shippingZipCode,
        shippingCountry: formValue.shippingCountry,
        billingSameAsShipping: formValue.billingSameAsShipping,
        billingAddressLine1: formValue.billingAddressLine1,
        billingAddressLine2: formValue.billingAddressLine2,
        billingCity: formValue.billingCity,
        billingState: formValue.billingState,
        billingZipCode: formValue.billingZipCode,
        billingCountry: formValue.billingCountry,
        shippingMethod: formValue.shippingMethod,
        specialInstructions: formValue.specialInstructions,
        isGift: formValue.isGift,
        giftMessage: formValue.giftMessage,
        newsletterSubscription: formValue.newsletterSubscription,
        smsUpdates: formValue.smsUpdates
      };

      // Create Payment Intent
      console.log('🔍 Creating payment intent with request:', paymentIntentRequest);
      const paymentIntentResponse = await this.checkoutService.createPaymentIntent(paymentIntentRequest).toPromise();
      
      console.log('🔍 Payment intent response:', paymentIntentResponse);
      
      if (!paymentIntentResponse?.success) {
        const errorMsg = paymentIntentResponse?.message || 'Failed to create payment intent';
        console.error('❌ Payment intent creation failed:', errorMsg);
        throw new Error(errorMsg);
      }

      const clientSecret = paymentIntentResponse.data?.clientSecret;
      if (!clientSecret) {
        throw new Error('Failed to get payment client secret');
      }

      // Confirm payment with Stripe
      console.log('🔍 Confirming payment with Stripe...');
      console.log('🔍 Client secret:', clientSecret);
      console.log('🔍 Billing details:', {
        name: formValue.customerName,
        email: formValue.customerEmail,
        phone: formValue.phoneNumber
      });

      const { error, paymentIntent } = await this.stripe.confirmCardPayment(clientSecret, {
        payment_method: {
          card: this.cardElement,
          billing_details: {
            name: formValue.customerName,
            email: formValue.customerEmail,
            phone: formValue.phoneNumber,
            address: {
              line1: formValue.billingSameAsShipping ? formValue.shippingAddressLine1 : formValue.billingAddressLine1,
              line2: formValue.billingSameAsShipping ? formValue.shippingAddressLine2 : formValue.billingAddressLine2,
              city: formValue.billingSameAsShipping ? formValue.shippingCity : formValue.billingCity,
              state: formValue.billingSameAsShipping ? formValue.shippingState : formValue.billingState,
              postal_code: formValue.billingSameAsShipping ? formValue.shippingZipCode : formValue.billingZipCode,
              country: formValue.billingSameAsShipping ? formValue.shippingCountry : formValue.billingCountry,
            },
          },
        }
      });

      console.log('🔍 Stripe payment result:', { error, paymentIntent });

      if (error) {
        console.error('❌ Stripe payment error:', error);
        throw new Error(`Payment failed: ${error.message}`);
      }

      if (paymentIntent.status !== 'succeeded') {
        throw new Error('Payment was not successful. Please try again.');
      }

      // Confirm payment and create order in backend using CheckoutService
      console.log('🔍 Confirming payment and creating order...');
      const confirmPaymentRequest = {
        paymentIntentId: paymentIntent.id,
        paymentMethodId: '',
        customerEmail: formValue.customerEmail,
        customerName: formValue.customerName,
        phoneNumber: formValue.phoneNumber,
        companyName: formValue.companyName || '',
        shippingAddressLine1: formValue.shippingAddressLine1,
        shippingAddressLine2: formValue.shippingAddressLine2 || '',
        shippingCity: formValue.shippingCity,
        shippingState: formValue.shippingState,
        shippingZipCode: formValue.shippingZipCode,
        shippingCountry: formValue.shippingCountry,
        billingSameAsShipping: formValue.billingSameAsShipping,
        billingAddressLine1: formValue.billingAddressLine1 || '',
        billingAddressLine2: formValue.billingAddressLine2 || '',
        billingCity: formValue.billingCity || '',
        billingState: formValue.billingState || '',
        billingZipCode: formValue.billingZipCode || '',
        billingCountry: formValue.billingCountry || '',
        shippingMethod: formValue.shippingMethod,
        specialInstructions: formValue.specialInstructions || '',
        isGift: formValue.isGift,
        giftMessage: formValue.giftMessage || ''
      };

      const orderResponse = await this.checkoutService.confirmPayment(confirmPaymentRequest).toPromise();
      
      if (!orderResponse?.success) {
        throw new Error('Payment successful but order creation failed. Please contact support with payment ID: ' + paymentIntent.id);
      }

      console.log('✅ Order created successfully:', orderResponse.data);

      // Clear cart
      try {
        await this.cartService.clearCart().toPromise();
      } catch (cartError) {
        console.warn('Failed to clear cart, but order was successful:', cartError);
      }

      // Show success notification
      this.notificationService.success('Order placed successfully!', 'Payment completed');
      
      // Redirect to orders page
      this.router.navigate(['/orders']);
      
    } catch (error: any) {
      console.error('Checkout error:', error);
      this.errorMessage = error.message || 'Payment failed. Please try again.';
      this.notificationService.error(this.errorMessage, 'Payment Failed');
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

  // Utility Methods
  formatPrice(price: number): string {
    return new Intl.NumberFormat('en-PK', {
      style: 'currency',
      currency: 'PKR'
    }).format(price);
  }

  getEstimatedDeliveryDate(days: number): string {
    const date = new Date();
    date.setDate(date.getDate() + days);
    return date.toLocaleDateString('en-US', { 
      weekday: 'long', 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  }
}