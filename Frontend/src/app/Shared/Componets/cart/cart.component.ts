import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../../Core/Services/cart.service';
import { GetCart, GetCartItem } from '../../../Core/Models/cart.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cart: GetCart | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.isLoading = true;
    this.cartService.getCart().subscribe({
      next: (cart) => {
        this.cart = cart;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load cart';
        this.isLoading = false;
        console.error('Error loading cart:', error);
      }
    });
  }

  updateQuantity(cartItemId: string, newQuantity: number): void {
    if (newQuantity <= 0) {
      this.removeItem(cartItemId);
      return;
    }

    this.cartService.updateCartItem({
      cartItemId,
      quantity: newQuantity
    }).subscribe({
      next: () => {
        this.loadCart();
      },
      error: (error) => {
        this.errorMessage = 'Failed to update quantity';
        console.error('Error updating quantity:', error);
      }
    });
  }

  removeItem(cartItemId: string): void {
    if (confirm('Are you sure you want to remove this item?')) {
      this.cartService.removeCartItem(cartItemId).subscribe({
        next: () => {
          this.loadCart();
        },
        error: (error) => {
          this.errorMessage = 'Failed to remove item';
          console.error('Error removing item:', error);
        }
      });
    }
  }

  clearCart(): void {
    if (confirm('Are you sure you want to clear your entire cart?')) {
      this.cartService.clearCart().subscribe({
        next: () => {
          this.cart = null;
          this.loadCart();
        },
        error: (error) => {
          this.errorMessage = 'Failed to clear cart';
          console.error('Error clearing cart:', error);
        }
      });
    }
  }

  continueShopping(): void {
    // Navigate to products page
    window.location.href = '/';
  }

  checkout(): void {
    // TODO: Implement checkout functionality
    alert('Checkout functionality coming soon!');
  }
}
