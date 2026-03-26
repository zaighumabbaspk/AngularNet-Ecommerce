import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../Core/Services/order.service';
import { GetOrder, OrderStatusLabels, OrderStatusColors } from '../../../Core/Models/order.model';

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.css'
})
export class OrderDetailsComponent implements OnInit {
  order: GetOrder | null = null;
  isLoading = true;
  errorMessage = '';
  orderStatusLabels = OrderStatusLabels;
  orderStatusColors = OrderStatusColors;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    if (orderId) {
      this.loadOrderDetails(orderId);
    } else {
      this.router.navigate(['/orders']);
    }
  }

  private loadOrderDetails(orderId: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.orderService.getOrder(orderId).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.data) {
          this.order = response.data;
        } else {
          this.errorMessage = response.message || 'Failed to load order details';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'An error occurred while loading order details';
        console.error('Order details loading error:', error);
      }
    });
  }

  goBackToOrders(): void {
    this.router.navigate(['/orders']);
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

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }
}
