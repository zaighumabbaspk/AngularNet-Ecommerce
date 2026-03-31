import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StripeService } from '../../../Core/Services/stripe.service';
import { CheckoutService } from '../../../Core/Services/checkout.service';
import { CreatePaymentIntentRequest, ConfirmPaymentRequest } from '../../../Core/Models/checkout.model';
import { environment } from '../../../environment/environment';

@Component({
  selector: 'app-stripe-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './stripe-checkout.component.html',
  styleUrl: './stripe-checkout.component.css'
})
export class StripeCheckoutComponent implements OnInit, OnDestroy, AfterViewInit {
  checkoutForm!: FormGroup;
  isLoading = false;
  errorMessage = '';
  paymentProcessing = false;
  clientSecret = '';
  paymentIntentId = '';

  constructor(
    private fb: FormBuilder,
    private stripeService: StripeService,
    private checkoutService: CheckoutService,
    public router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    this.initializeForm();
    await this.initializeStripe();
  }

  async ngAfterViewInit(): Promise<void> {
    await this.mountCardElement();
  }

  ngOnDestroy(): void {
    this.stripeService.destroyCardElement();
  }

  private initializeForm(): void {
    this.checkoutForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      name: ['', [Validators.required]],
      shippingAddress: ['', [Validators.required, Validators.minLength(10)]],
      billingAddress: ['', [Validators.required, Validators.minLength(10)]],
      sameAsShipping: [false]
    });

    // Watch for same as shipping checkbox
    this.checkoutForm.get('sameAsShipping')?.valueChanges.subscribe(value => {
      if (value) {
        const shippingAddress = this.checkoutForm.get('shippingAddress')?.value;
        this.checkoutForm.patchValue({ billingAddress: shippingAddress });
      }
    });
  }

  private async initializeStripe(): Promise<void> {
    try {
      
      // Check if we have a valid key
      if (!environment.stripePublishableKey || environment.stripePublishableKey.includes('your_publishable_key_here')) {
        throw new Error('Invalid Stripe publishable key. Please set your actual Stripe key in environment.ts');
      }
      
      await this.stripeService.initializeStripe();
      
      const cardElement = this.stripeService.createCardElement();
      if (!cardElement) {
        throw new Error('Failed to create card element');
      }
      
    } catch (error) {
      console.error('Stripe initialization failed:', error);
      this.errorMessage = `Failed to initialize payment form: ${error}`;
    }
  }

  private async mountCardElement(): Promise<void> {
    try {
      console.log('Attempting to mount card element...');
      const cardContainer = document.getElementById('card-element');
      if (cardContainer) {
        this.stripeService.mountCardElement('card-element');
        console.log(' Card element mounted successfully');
        
        // Hide loading message after successful mount
        setTimeout(() => {
          const loadingElement = document.querySelector('.card-element-loading');
          if (loadingElement) {
            (loadingElement as HTMLElement).style.display = 'none';
          }
        }, 1000);
        
      } else {
        console.error('Card element container not found');
        this.errorMessage = 'Payment form failed to load. Please refresh the page.';
      }
    } catch (error) {
      console.error('Card element mounting failed:', error);
      this.errorMessage = `Failed to mount payment form: ${error}`;
    }
  }

  async onSubmit(): Promise<void> {
    if (!this.checkoutForm.valid) {
      this.markFormGroupTouched();
      return;
    }

    this.paymentProcessing = true;
    this.errorMessage = '';

    try {
      // Step 1: Create Payment Intent
      const paymentIntentRequest: CreatePaymentIntentRequest = {
        shippingAddress: this.checkoutForm.value.shippingAddress,
        billingAddress: this.checkoutForm.value.billingAddress,
        customerEmail: this.checkoutForm.value.email,
        customerName: this.checkoutForm.value.name
      };

      const paymentIntentResponse = await this.checkoutService.createPaymentIntent(paymentIntentRequest).toPromise();
      
      if (!paymentIntentResponse?.success || !paymentIntentResponse.data) {
        throw new Error(paymentIntentResponse?.message || 'Failed to create payment intent');
      }

      this.clientSecret = paymentIntentResponse.data.clientSecret;
      this.paymentIntentId = paymentIntentResponse.data.paymentIntentId;

      // Step 2: Create Payment Method
      const billingDetails = {
        name: this.checkoutForm.value.name,
        email: this.checkoutForm.value.email,
        address: {
          line1: this.checkoutForm.value.billingAddress,
        },
      };

      const paymentMethod = await this.stripeService.createPaymentMethod(billingDetails);

      // Step 3: Confirm Payment
      const paymentIntent = await this.stripeService.confirmCardPayment(
        this.clientSecret, 
        paymentMethod.id
      );

      if (paymentIntent.status === 'succeeded') {
        const confirmRequest: ConfirmPaymentRequest = {
          paymentIntentId: this.paymentIntentId,
          paymentMethodId: paymentMethod.id,
          shippingAddress: this.checkoutForm.value.shippingAddress,
          billingAddress: this.checkoutForm.value.billingAddress
        };

        const confirmResponse = await this.checkoutService.confirmPayment(confirmRequest).toPromise();
        
        if (confirmResponse?.success) {
          this.router.navigate(['/orders', confirmResponse.data?.id]);
        } else {
          throw new Error(confirmResponse?.message || 'Failed to confirm payment');
        }
      } else {
        throw new Error('Payment was not successful');
      }

    } catch (error: any) {
      this.errorMessage = error.message || 'Payment failed. Please try again.';
      console.error('Payment error:', error);
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

  isFieldInvalid(fieldName: string): boolean {
    const field = this.checkoutForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.checkoutForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['email']) return 'Please enter a valid email';
      if (field.errors['minlength']) return `${fieldName} must be at least ${field.errors['minlength'].requiredLength} characters`;
    }
    return '';
  }
}
