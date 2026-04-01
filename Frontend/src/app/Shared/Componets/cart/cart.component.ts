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
    
    // Check if user is authenticated
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: '/checkout' } 
      });
      return;
    }

    // Check if cart has items
    if (!this.cart || this.cart.cartItems.length === 0) {
      this.errorMessage = 'Your cart is empty';
      return;
    }


    this.router.navigate(['/checkout']).then(success => {
    }).catch(error => {
      console.error(' Navigation failed:', error);
    });
  }
}
