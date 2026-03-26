import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-checkout-cancel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './checkout-cancel.component.html',
  styleUrl: './checkout-cancel.component.css'
})
export class CheckoutCancelComponent {
  constructor(private router: Router) {}

  goToCart(): void {
    this.router.navigate(['/cart']);
  }

  continueShopping(): void {
    this.router.navigate(['/']);
  }
}
