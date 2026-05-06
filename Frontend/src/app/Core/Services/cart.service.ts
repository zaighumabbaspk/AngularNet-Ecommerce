import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, of, throwError, tap, switchMap } from 'rxjs';
import { environment } from '../../environment/environment';
import { AddToCartRequest, UpdateCartItemRequest, GetCart, GetCartItem } from '../Models/cart.model';
import { AuthService } from './auth.service';
import { ProductService } from './product.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = `${environment.apiBaseUrl}/cart`;
  private guestCartKey = 'guest_cart';
  
  private cartSubject = new BehaviorSubject<GetCart | null>(null);
  public cart$ = this.cartSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private productService: ProductService
  ) {
    // Initialize cart on service creation
    this.initializeCart();
    (window as any).cartServiceInstance = this;
  }

  private initializeCart(): void {
    this.loadGuestCart();
       // If user is authenticated, try to load from backend
    if (this.authService.isAuthenticated()) {
      this.loadCart();
    }
  }

  loadCart(): void {
    if (!this.authService.isAuthenticated()) {
      this.loadGuestCart();
      return;
    }

    this.getCart().pipe(
      catchError((error) => {
        console.error('Error loading authenticated cart:', error);
        this.loadGuestCart();
        return of(null);
      })
    ).subscribe({
      next: (cart) => {
        if (cart) {
          this.cartSubject.next(cart);
        }
      }
    });
  }

  // Load guest cart from localStorage
  private loadGuestCart(): void {
    const guestCartData = localStorage.getItem(this.guestCartKey);
    if (guestCartData) {
      try {
        const guestCart: GetCart = JSON.parse(guestCartData);
        this.cartSubject.next(guestCart);
      } catch (error) {
        console.error('Error parsing guest cart:', error);
        this.createEmptyGuestCart();
      }
    } else {
      this.createEmptyGuestCart();
    }
  }

  // Create empty guest cart
  private createEmptyGuestCart(): void {
    const emptyCart: GetCart = {
      id: 'guest-cart',
      userId: 'guest',
      cartItems: [],
      total: 0,
      totalItems: 0
    };
    this.cartSubject.next(emptyCart);
    this.saveGuestCart(emptyCart);
  }

  private saveGuestCart(cart: GetCart): void {
    localStorage.setItem(this.guestCartKey, JSON.stringify(cart));
  }

  reloadCart(): Observable<GetCart> {
    if (this.authService.isAuthenticated()) {
      return this.getCart().pipe(
        tap((cart) => this.cartSubject.next(cart)),
        catchError((error) => {
          console.error('Error reloading cart:', error);
          return throwError(() => error);
        })
      );
    } else {
      // For guest users, just reload from localStorage
      this.loadGuestCart();
      const currentCart = this.cartSubject.value;
      return of(currentCart || this.createEmptyGuestCartSync());
    }
  }

  getCart(): Observable<GetCart> {
    return this.http.get<GetCart>(this.apiUrl);
  }

  // Add item to cart (works for both authenticated and guest users)
  addToCart(request: AddToCartRequest): Observable<any> {
    
    if (this.authService.isAuthenticated()) {
      // Authenticated user - use backend
      console.log('🔍 Adding to authenticated user cart via backend');
      return this.http.post(`${this.apiUrl}/items`, request).pipe(
        tap(() => {
          this.loadCart(); // Reload cart after adding
        }),
        catchError(error => {
          console.error('❌ CartService.addToCart backend error:', error);
          return throwError(() => error);
        })
      );
    } else {
      // Guest user - use localStorage
      console.log('🔍 Adding to guest cart via localStorage');
      return this.addToGuestCart(request);
    }
  }

  private addToGuestCart(request: AddToCartRequest): Observable<any> {

    return this.productService.getProductById(request.productId).pipe(
      switchMap(productResponse => {
        return new Observable(observer => {
          try {
            const product = productResponse.data || productResponse;
            const currentCart = this.cartSubject.value || this.createEmptyGuestCartSync();
            
            // Check if item already exists in cart
            const existingItemIndex = currentCart.cartItems.findIndex(
              item => item.productId === request.productId
            );

            if (existingItemIndex >= 0) {
              // Update quantity of existing item
              currentCart.cartItems[existingItemIndex].quantity += request.quantity;
              currentCart.cartItems[existingItemIndex].subtotal = 
                currentCart.cartItems[existingItemIndex].quantity * currentCart.cartItems[existingItemIndex].price;
            } else {
              // Add new item with product details
              const newItem: GetCartItem = {
                id: `guest-item-${Date.now()}`,
                productId: request.productId,
                productName: product.name || 'Product',
                productImage: product.image || '',
                imageUrl: product.image || '',
                productPrice: product.price || 0,
                price: product.price || 0,
                categoryName: product.categoryName || '',
                quantity: request.quantity,
                subtotal: (product.price || 0) * request.quantity,
                availableStock: product.quantity || 999
              };
              currentCart.cartItems.push(newItem);
            }

            // Recalculate totals
            currentCart.total = currentCart.cartItems.reduce((sum, item) => sum + item.subtotal, 0);
            currentCart.totalItems = currentCart.cartItems.reduce((sum, item) => sum + item.quantity, 0);

            // Save and update
            this.saveGuestCart(currentCart);
            this.cartSubject.next(currentCart);

            observer.next({ success: true, message: 'Item added to cart' });
            observer.complete();
          } catch (error) {
            observer.error(error);
          }
        });
      }),
      catchError(error => {
        console.error('Error fetching product details:', error);
        // Fallback: add item without full product details
        return new Observable(observer => {
          try {
            const currentCart = this.cartSubject.value || this.createEmptyGuestCartSync();
            
            const newItem: GetCartItem = {
              id: `guest-item-${Date.now()}`,
              productId: request.productId,
              productName: 'Product',
              productImage: '',
              imageUrl: '',
              productPrice: 0,
              price: 0,
              categoryName: '',
              quantity: request.quantity,
              subtotal: 0,
              availableStock: 999
            };
            currentCart.cartItems.push(newItem);

            currentCart.totalItems = currentCart.cartItems.reduce((sum, item) => sum + item.quantity, 0);

            this.saveGuestCart(currentCart);
            this.cartSubject.next(currentCart);

            observer.next({ success: true, message: 'Item added to cart' });
            observer.complete();
          } catch (error) {
            observer.error(error);
          }
        });
      })
    );
  }

  private createEmptyGuestCartSync(): GetCart {
    return {
      id: 'guest-cart',
      userId: 'guest',
      cartItems: [],
      total: 0,
      totalItems: 0
    };
  }

  // Update cart item quantity
  updateCartItem(request: UpdateCartItemRequest): Observable<any> {
    if (this.authService.isAuthenticated()) {
      return this.http.put(`${this.apiUrl}/items`, request);
    } else {
      return this.updateGuestCartItem(request);
    }
  }

  private updateGuestCartItem(request: UpdateCartItemRequest): Observable<any> {
    return new Observable(observer => {
      try {
        const currentCart = this.cartSubject.value;
        if (!currentCart) {
          observer.error('Cart not found');
          return;
        }

        const itemIndex = currentCart.cartItems.findIndex(item => item.id === request.cartItemId);
        if (itemIndex >= 0) {
          currentCart.cartItems[itemIndex].quantity = request.quantity;
          currentCart.cartItems[itemIndex].subtotal = 
            currentCart.cartItems[itemIndex].quantity * currentCart.cartItems[itemIndex].price;

          // Recalculate totals
          currentCart.total = currentCart.cartItems.reduce((sum, item) => sum + item.subtotal, 0);
          currentCart.totalItems = currentCart.cartItems.reduce((sum, item) => sum + item.quantity, 0);

          this.saveGuestCart(currentCart);
          this.cartSubject.next(currentCart);
        }

        observer.next({ success: true });
        observer.complete();
      } catch (error) {
        observer.error(error);
      }
    });
  }

  // Remove item from cart
  removeCartItem(cartItemId: string): Observable<any> {
    if (this.authService.isAuthenticated()) {
      return this.http.delete(`${this.apiUrl}/items/${cartItemId}`);
    } else {
      return this.removeGuestCartItem(cartItemId);
    }
  }

  private removeGuestCartItem(cartItemId: string): Observable<any> {
    return new Observable(observer => {
      try {
        const currentCart = this.cartSubject.value;
        if (!currentCart) {
          observer.error('Cart not found');
          return;
        }

        currentCart.cartItems = currentCart.cartItems.filter(item => item.id !== cartItemId);
        
        // Recalculate totals
        currentCart.total = currentCart.cartItems.reduce((sum, item) => sum + item.subtotal, 0);
        currentCart.totalItems = currentCart.cartItems.reduce((sum, item) => sum + item.quantity, 0);

        this.saveGuestCart(currentCart);
        this.cartSubject.next(currentCart);

        observer.next({ success: true });
        observer.complete();
      } catch (error) {
        observer.error(error);
      }
    });
  }

  // Clear entire cart
  clearCart(): Observable<any> {
    if (this.authService.isAuthenticated()) {
      return this.http.delete(this.apiUrl);
    } else {
      return this.clearGuestCart();
    }
  }

  private clearGuestCart(): Observable<any> {
    return new Observable(observer => {
      try {
        this.createEmptyGuestCart();
        observer.next({ success: true });
        observer.complete();
      } catch (error) {
        observer.error(error);
      }
    });
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

  // Transfer guest cart to authenticated user (call this after login)
  transferGuestCartToUser(): void {
    const guestCart = this.cartSubject.value;
    if (guestCart && guestCart.cartItems.length > 0 && this.authService.isAuthenticated()) {
      // Transfer each item to the authenticated user's cart
      guestCart.cartItems.forEach(item => {
        const request: AddToCartRequest = {
          productId: item.productId,
          quantity: item.quantity
        };
        
        this.http.post(`${this.apiUrl}/items`, request).subscribe({
          next: () => console.log(`Transferred ${item.productName} to user cart`),
          error: (error) => console.error('Error transferring item:', error)
        });
      });

      localStorage.removeItem(this.guestCartKey);
      setTimeout(() => this.loadCart(), 1000);
    }
  }

  getGuestCartItems(): GetCartItem[] {
    const cart = this.cartSubject.value;
    return cart ? cart.cartItems : [];
  }
}
