import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';

import { GuestCheckoutService } from '../../../Core/Services/guest-checkout.service';
import { CustomNotificationService } from '../../../Core/Services/custom-notification.service';
import { GetOrder } from '../../../Core/Models/order.model';

@Component({
  selector: 'app-guest-order-tracking',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './guest-order-tracking.component.html',
  styleUrls: ['./guest-order-tracking.component.css']
})
export class GuestOrderTrackingComponent implements OnInit {
  trackingForm!: FormGroup;
  isLoading = false;
  order: GetOrder | null = null;
  orderHistory: GetOrder[] = [];
  showOrderHistory = false;

  constructor(
    private fb: FormBuilder,
    private guestCheckoutService: GuestCheckoutService,
    private notificationService: CustomNotificationService,
    private route: ActivatedRoute
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    // Pre-fill form if coming from success page
    this.route.queryParams.subscribe(params => {
      if (params['email'] && params['orderNumber']) {
        this.trackingForm.patchValue({
          email: params['email'],
          orderNumber: params['orderNumber']
        });
        this.trackOrder();
      }
    });
  }

  private initializeForm(): void {
    this.trackingForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      orderNumber: ['', Validators.required]
    });
  }

  trackOrder(): void {
    if (this.trackingForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isLoading = true;
    const request = {
      email: this.trackingForm.get('email')?.value,
      orderNumber: this.trackingForm.get('orderNumber')?.value
    };

    this.guestCheckoutService.trackGuestOrder(request).subscribe({
      next: (response) => {
        if (response.flag) {
          this.order = response.data;
          this.notificationService.success('Order found! Displaying details.');
        } else {
          this.notificationService.error(response.message || 'Order not found. Please check your details and try again.');
          this.order = null;
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationService.error('Failed to track order');
        this.order = null;
        this.isLoading = false;
      }
    });
  }

  loadOrderHistory(): void {
    const email = this.trackingForm.get('email')?.value;
    if (!email) return;

    this.isLoading = true;
    this.guestCheckoutService.getGuestOrdersByEmail(email).subscribe({
      next: (response) => {
        if (response.flag) {
          this.orderHistory = response.data;
          this.showOrderHistory = true;
          this.notificationService.success(`Found ${this.orderHistory.length} orders`);
        } else {
          this.notificationService.error('No orders found for this email');
          this.orderHistory = [];
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationService.error('Failed to load order history');
        this.orderHistory = [];
        this.isLoading = false;
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.trackingForm.controls).forEach(key => {
      this.trackingForm.get(key)?.markAsTouched();
    });
  }

  getStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'pending': return '#f39c12';
      case 'confirmed': return '#3498db';
      case 'processing': return '#9b59b6';
      case 'shipped': return '#e67e22';
      case 'delivered': return '#27ae60';
      case 'cancelled': return '#e74c3c';
      default: return '#95a5a6';
    }
  }

  getStatusIcon(status: string): string {
    switch (status?.toLowerCase()) {
      case 'pending': return 'fas fa-clock';
      case 'confirmed': return 'fas fa-check-circle';
      case 'processing': return 'fas fa-cog fa-spin';
      case 'shipped': return 'fas fa-truck';
      case 'delivered': return 'fas fa-box-open';
      case 'cancelled': return 'fas fa-times-circle';
      default: return 'fas fa-question-circle';
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}