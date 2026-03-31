import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CheckoutService } from '../../../Core/Services/checkout.service';
import { CreateCheckoutSessionRequest } from '../../../Core/Models/checkout.model';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit {
  checkoutForm!: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private checkoutService: CheckoutService,
    public router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.checkoutForm = this.fb.group({
      shippingAddress: ['', [Validators.required, Validators.minLength(10)]],
      billingAddress: ['', [Validators.required, Validators.minLength(10)]],
      sameAsShipping: [false]
    });

    this.checkoutForm.get('sameAsShipping')?.valueChanges.subscribe(value => {
      if (value) {
        const shippingAddress = this.checkoutForm.get('shippingAddress')?.value;
        this.checkoutForm.patchValue({ billingAddress: shippingAddress });
      }
    });
  }

  onSubmit(): void {
    if (this.checkoutForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const request: CreateCheckoutSessionRequest = {
        shippingAddress: this.checkoutForm.value.shippingAddress,
        billingAddress: this.checkoutForm.value.billingAddress
      };

      this.checkoutService.createCheckoutSession(request).subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success && response.data) {
            // Redirect to Stripe checkout
            this.checkoutService.redirectToStripe(response.data.url);
          } else {
            this.errorMessage = response.message || 'Failed to create checkout session';
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'An error occurred during checkout';
          console.error('Checkout error:', error);
        }
      });
    } else {
      this.markFormGroupTouched();
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
      if (field.errors['minlength']) return `${fieldName} must be at least ${field.errors['minlength'].requiredLength} characters`;
    }
    return '';
  }
}
