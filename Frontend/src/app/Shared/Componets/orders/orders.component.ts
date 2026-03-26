import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { OrderService } from '../../../Core/Services/order.service';
import { OrderSummary, OrderStatusLabels, OrderStatusColors } from '../../../Core/Models/order.model';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class OrdersComponent implements OnInit {
  orders: OrderSummary[] = [];
  isLoading = true;
  errorMessage = '';
  orderStatusLabels = OrderStatusLabels;
  orderStatusColors = OrderStatusColors;

  constructor(
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    console.log('🔍 Orders component initialized');
    this.loadOrders();
  }

  public loadOrders(): void {
    console.log('🔍 Loading orders...');
    this.isLoading = true;
    this.errorMessage = '';

    this.orderService.getMyOrders().subscribe({
      next: (response) => {
        console.log('🔍 Orders API Response:', response);
        this.isLoading = false;
        if (response.success && response.data) {
          this.orders = response.data;
          console.log('✅ Orders loaded successfully:', this.orders.length);
        } else {
          this.errorMessage = response.message || 'Failed to load orders';
          console.log('❌ Orders API returned error:', this.errorMessage);
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'An error occurred while loading orders';
        console.error('❌ Orders loading error:', error);
        console.log('🔍 Error details:', {
          status: error.status,
          statusText: error.statusText,
          url: error.url,
          message: error.message
        });
      }
    });
  }

  viewOrderDetails(orderId: string): void {
    this.router.navigate(['/orders', orderId]);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  continueShopping(): void {
    this.router.navigate(['/']);
  }
}
