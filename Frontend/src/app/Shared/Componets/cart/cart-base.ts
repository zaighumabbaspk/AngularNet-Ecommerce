import { Directive, OnInit } from '@angular/core';
import { GetCart } from '../../../Core/Models/cart.model';
import { CartService } from '../../../Core/Services/cart.service';

@Directive()
export abstract class CartBase implements OnInit {
  cart: GetCart | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(protected cartService: CartService) {}

  ngOnInit(): void {
    // Subscribe to cart changes
    this.cartService.cart$.subscribe(cart => {
      this.cart = cart;
      this.isLoading = false;
    });

  }

  public loadCart(): void {
    this.isLoading = true;
    
    const currentCart = this.cartService.getCurrentCart();
    if (currentCart) {
      this.cart = currentCart;
      this.isLoading = false;
      return;
    }

    this.cartService.reloadCart().subscribe({
      next: (cart) => {
        this.cart = cart;
        this.isLoading = false;
      },
      error: (error) => {
        this.cart = this.cartService.getCurrentCart();
        this.isLoading = false;
        console.log('Using local cart for guest user');
      }
    });
  }

  public updateQuantity(cartItemId: string, newQuantity: number): void {
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
        this.errorMessage = 'Failed to update quantity';
        this.isLoading = false;
        console.error('Error updating quantity:', error);
      }
    });
  }

  public removeItem(cartItemId: string): void {
    this.isLoading = true;
    this.cartService.removeCartItem(cartItemId).subscribe({
      next: () => {
        this.loadCart();
      },
      error: (error) => {
        this.errorMessage = 'Failed to remove item';
        this.isLoading = false;
        console.error('Error removing item:', error);
      }
    });
  }

  public clearCart(): void {
    this.isLoading = true;
    this.cartService.clearCart().subscribe({
      next: () => {
        this.cart = null;
        this.loadCart();
      },
      error: (error) => {
        this.errorMessage = 'Failed to clear cart';
        this.isLoading = false;
        console.error('Error clearing cart:', error);
      }
    });
  }

  public parseQuantity(value: any): number {
    return parseInt(value, 10);
  }

  public onQuantityChange(cartItemId: string, event: any): void {
    const newQuantity = this.parseQuantity(event.target.value);
    this.updateQuantity(cartItemId, newQuantity);
  }

  public continueShopping(): void {
    window.location.href = '/';
  }

  public checkout(): void {
  }
}
