import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-checkout-success',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './checkout-success.component.html',
  styleUrl: './checkout-success.component.css'
})
export class CheckoutSuccessComponent implements OnInit {
  orderId: string | null = null;
  paymentIntentId: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.orderId = this.route.snapshot.queryParamMap.get('orderId');
    this.paymentIntentId = this.route.snapshot.queryParamMap.get('paymentIntentId');
  }

  goToOrders(): void {
    this.router.navigate(['/orders']);
  }

  viewOrderDetails(): void {
    if (this.orderId) {
      this.router.navigate(['/orders', this.orderId]);
    } else {
      this.goToOrders();
    }
  }

  continueShopping(): void {
    this.router.navigate(['/']);
  }
}
