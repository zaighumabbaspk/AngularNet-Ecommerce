import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CartService } from '../../../Core/Services/cart.service';
import { CartDrawerService } from '../../../Core/Services/cart-drawer.service';
import { AuthService } from '../../../Core/Services/auth.service';
import { CustomNotificationService } from '../../../Core/Services/custom-notification.service';
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
    private notification: CustomNotificationService
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
        this.notification.error('Failed to update quantity', 'Cart Error');
        this.isLoading = false;
        console.error('Error updating quantity:', error);
      }
    });
  }

  override removeItem(cartItemId: string): void {
    this.cartService.removeCartItem(cartItemId).subscribe({
      next: () => {
        this.notification.cartSuccess('Item removed from cart', 'Cart Updated');
        this.loadCart();
      },
      error: (error) => {
        this.notification.error('Failed to remove item', 'Cart Error');
        console.error('Error removing item:', error);
      }
    });
  }

  override clearCart(): void {
    if (confirm('Are you sure you want to clear your entire cart?')) {
      super.clearCart();
      this.notification.cartSuccess('Cart cleared successfully', 'Cart Updated');
    }
  }

  closeDrawer(): void {
    this.cartDrawerService.closeDrawer();
  }

  override checkout(): void {
    console.log('🔍 Cart drawer checkout clicked');
    
    // Check if user is authenticated
    if (!this.authService.isAuthenticated()) {
      console.log('🔍 User not authenticated, redirecting to login');
      this.closeDrawer();
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: '/checkout' } 
      });
      return;
    }

    // Check if cart has items
    if (!this.cart || this.cart.cartItems.length === 0) {
      console.log('❌ Cart is empty');
      this.notification.warning('Your cart is empty', 'Cannot Checkout');
      return;
    }

    console.log('✅ Navigating to checkout from drawer');
    this.closeDrawer();
    
    // Navigate to checkout page
    this.router.navigate(['/checkout']).then(success => {
      console.log('🔍 Navigation result:', success);
      if (success) {
        this.notification.success('Redirecting to checkout...', 'Success');
      }
    }).catch(error => {
      console.error('❌ Navigation failed:', error);
      this.notification.error('Failed to navigate to checkout', 'Navigation Error');
    });
  }

  override continueShopping(): void {
    this.closeDrawer();
  }
}
