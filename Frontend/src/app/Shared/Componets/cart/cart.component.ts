import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../../../Core/Services/cart.service';
import { AuthService } from '../../../Core/Services/auth.service';
import { CartBase } from '../cart/cart-base';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent extends CartBase {
  constructor(
    cartService: CartService,
    private router: Router,
    private authService: AuthService
  ) {
    super(cartService);
  }

  override checkout(): void {
    // Check if cart has items
    if (!this.cart || this.cart.cartItems.length === 0) {
      this.errorMessage = 'Your cart is empty';
      return;
    }

    // Show checkout options modal or redirect based on authentication
    if (this.authService.isAuthenticated()) {
      // Authenticated user - go to regular checkout
      this.router.navigate(['/checkout']).then(success => {
      }).catch(error => {
        console.error(' Navigation failed:', error);
      });
    } else {
      // Guest user - show checkout options
      this.showCheckoutOptions();
    }
  }

  showCheckoutOptions(): void {
    // For now, redirect to guest checkout directly
    // In future, you could show a modal with options: "Login" or "Continue as Guest"
    this.router.navigate(['/guest-checkout']);
  }
}
