import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartService } from '../../../Core/Services/cart.service';
import { CartDrawerService } from '../../../Core/Services/cart-drawer.service';
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
    this.closeDrawer();
    // TODO: Navigate to checkout page
    this.toastr.info('Checkout functionality coming soon!', 'Info');
  }

  override continueShopping(): void {
    this.closeDrawer();
  }
}
