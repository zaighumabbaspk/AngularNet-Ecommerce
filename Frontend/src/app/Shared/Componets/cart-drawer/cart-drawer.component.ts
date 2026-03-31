import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CartService } from '../../../Core/Services/cart.service';
import { CartDrawerService } from '../../../Core/Services/cart-drawer.service';
import { AuthService } from '../../../Core/Services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { CartBase } from '../cart/cart-base';

@Component({
  selector: 'app-cart-drawer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart-drawer.component.html',
  styleUrls: ['./cart-drawer.component.css']
})
export class CartDrawerComponent extends CartBase {
  isOpen = false;

  constructor(
    cartService: CartService,
    private cartDrawerService: CartDrawerService,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {
    super(cartService);
  }

  override ngOnInit(): void {
    super.ngOnInit();

    this.cartDrawerService.isOpen$.subscribe(isOpen => {
      this.isOpen = isOpen;
      if (isOpen) {
        this.loadCart();
      }
    });
  }

  override updateQuantity(cartItemId: string, newQuantity: number): void {
    if (newQuantity <= 0) {
      this.removeItem(cartItemId);
      return;
    }

    this.isLoading = true;
    this.cartService.updateCartItem({ cartItemId, quantity: newQuantity }).subscribe({
      next: () => {
        this.loadCart();
      },
      error: (error) => {
        this.toastr.error('Failed to update quantity', 'Error');
        this.isLoading = false;
        console.error('Error updating quantity:', error);
      }
    });
  }

  override removeItem(cartItemId: string): void {
    this.cartService.removeCartItem(cartItemId).subscribe({
      next: () => {
        this.toastr.success('Item removed from cart', 'Success');
        this.loadCart();
      },
      error: (error) => {
        this.toastr.error('Failed to remove item', 'Error');
        console.error('Error removing item:', error);
      }
    });
  }

  override clearCart(): void {
    if (confirm('Are you sure you want to clear your entire cart?')) {
      super.clearCart();
      this.toastr.success('Cart cleared', 'Success');
    }
  }

  closeDrawer(): void {
    this.cartDrawerService.closeDrawer();
  }

  override checkout(): void {

    if (!this.authService.isAuthenticated()) {
      this.closeDrawer();
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: '/checkout' } 
      });
      return;
    }

    // Check if cart has items
    if (!this.cart || this.cart.cartItems.length === 0) {
      this.toastr.warning('Your cart is empty', 'Cannot Checkout');
      return;
    }
    this.closeDrawer();
    
    this.router.navigate(['/checkout']).then(success => {
      if (success) {
        this.toastr.success('Redirecting to checkout...', 'Success');
      }
    }).catch(error => {
      this.toastr.error('Failed to navigate to checkout', 'Error');
    });
  }

  override continueShopping(): void {
    this.closeDrawer();
  }
}
