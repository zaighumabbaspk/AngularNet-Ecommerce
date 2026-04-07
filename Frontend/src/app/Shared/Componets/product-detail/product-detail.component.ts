import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../Core/Services/product.service';
import { WishlistService } from '../../../Core/Services/wishlist.service';
import { RecentlyViewedService } from '../../../Core/Services/recently-viewed.service';
import { CartService } from '../../../Core/Services/cart.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CustomNotificationService } from '../../../Core/Services/custom-notification.service';
import { Product } from '../../../Core/Models/Product.model';
import { AuthService } from '../../../Core/Services/auth.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {

  product?: Product;
  isLoading: boolean = true;
  selectedQuantity: number = 1;
  isInWishlist: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private wishlistService: WishlistService,
    private recentlyViewedService: RecentlyViewedService,
    private cartService: CartService,
    public authService: AuthService,
    private notification: CustomNotificationService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.loadProduct(id);
      }
    });
  }

  loadProduct(id: string) {
    this.isLoading = true;

    this.productService.getProductById(id).subscribe({
      next: (res: Product) => {
        this.product = res;
        this.isLoading = false;
        
        // Track recently viewed if user is authenticated
        if (this.authService.isAuthenticated()) {
          this.trackRecentlyViewed(id);
          this.checkWishlistStatus(id);
        }
      },
      error: (err: any) => {
        console.error('Error loading product:', err);
        this.isLoading = false;
        this.notification.error(
          'Failed to load product details. Please try again.',
          'Error'
        );
      }
    });
  }

  trackRecentlyViewed(productId: string) {
    this.recentlyViewedService.addRecentlyViewed(productId).subscribe({
      next: () => {
        // Silently track - no user notification needed
      },
      error: (err: any) => {
        console.error('Failed to track recently viewed:', err);
      }
    });
  }

  checkWishlistStatus(productId: string) {
    this.wishlistService.isInWishlist(productId).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.isInWishlist = response.data;
        }
      },
      error: (err: any) => {
        console.error('Failed to check wishlist status:', err);
      }
    });
  }

  toggleWishlist() {
    if (!this.product) return;

    if (!this.authService.isAuthenticated()) {
      this.notification.loginRequired('Please login to manage your wishlist');
      return;
    }

    if (this.isInWishlist) {
      this.removeFromWishlist();
    } else {
      this.addToWishlist();
    }
  }

  addToWishlist() {
    if (!this.product) return;

    this.wishlistService.addToWishlist(this.product.id).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.isInWishlist = true;
          this.notification.success('Added to wishlist!', 'Success');
        }
      },
      error: (err: any) => {
        console.error('Failed to add to wishlist:', err);
        this.notification.error('Failed to add to wishlist', 'Error');
      }
    });
  }

  removeFromWishlist() {
    if (!this.product) return;

    this.wishlistService.removeFromWishlist(this.product.id).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.isInWishlist = false;
          this.notification.success('Removed from wishlist!', 'Success');
        }
      },
      error: (err: any) => {
        console.error('Failed to remove from wishlist:', err);
        this.notification.error('Failed to remove from wishlist', 'Error');
      }
    });
  }

  increaseQuantity() {
    if (this.product && this.selectedQuantity < this.product.quantity) {
      this.selectedQuantity++;
    } 
    else if (this.product && this.selectedQuantity >= this.product.quantity) {
      this.notification.warning(
        'Cannot exceed available stock',
        'Stock Limit'
      );
    }
  }

  addToCart() {
  if (!this.product) return;

  if (!this.authService.isAuthenticated()) {
    this.notification.loginRequired('Please login to add items to cart');
    return;
  }

  const request = {
    productId: this.product.id,
    quantity: this.selectedQuantity
  };

  this.cartService.addToCart(request).subscribe({
    next: () => {
      this.notification.cartSuccess(`${this.product?.name} added to cart!`, 'Success');
      this.cartService.loadCart();
    },
    error: () => {
      this.notification.error('Failed to add item to cart', 'Error');
    }
  });
}

  // addToCart() {
  //   if (!this.product) return;
  //   this.cartHelperService.addToCart(this.product, this.selectedQuantity);
  // }

  decreaseQuantity() {
    if (this.selectedQuantity > 1) {
      this.selectedQuantity--;
    } else {
      this.notification.warning(
        'Minimum quantity is 1',
        'Limit'
      );
    }
  }
}