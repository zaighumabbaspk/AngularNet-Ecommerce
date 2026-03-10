import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, of, throwError, tap } from 'rxjs';
import { environment } from '../../environment/environment';
import { AddToCartRequest, UpdateCartItemRequest, GetCart } from '../Models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = `${environment.apiBaseUrl}/cart`;
  
  private cartSubject = new BehaviorSubject<GetCart | null>(null);
  public cart$ = this.cartSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Load cart from backend and update cart$
  loadCart(): void {
    this.getCart().pipe(
      catchError((error) => {
        console.error('Error loading cart:', error);
        return of(null);
      })
    ).subscribe({
      next: (cart) => {
        this.cartSubject.next(cart);
      }
    });
  }


  reloadCart(): Observable<GetCart> {
    return this.getCart().pipe(
      tap((cart) => this.cartSubject.next(cart)),
      catchError((error) => {
        console.error('Error reloading cart:', error);
        return throwError(() => error);
      })
    );
  }

  // Get cart
  getCart(): Observable<GetCart> {
    return this.http.get<GetCart>(this.apiUrl);
  }

  // Add item to cart
  addToCart(request: AddToCartRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/items`, request);
  }

  // Update cart item quantity
  updateCartItem(request: UpdateCartItemRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/items`, request);
  }

  // Remove item from cart
  removeCartItem(cartItemId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/items/${cartItemId}`);
  }

  // Clear entire cart
  clearCart(): Observable<any> {
    return this.http.delete(this.apiUrl);
  }

  // Get current cart value
  getCurrentCart(): GetCart | null {
    return this.cartSubject.value;
  }

  // Get total items in cart
  getTotalItems(): number {
    const cart = this.cartSubject.value;
    return cart ? cart.totalItems : 0;
  }

  getTotalPrice(): number {
    const cart = this.cartSubject.value;
    return cart ? cart.total : 0;
  }
}
