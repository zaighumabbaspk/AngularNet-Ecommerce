import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../Core/Services/product.service';
import { WishlistService } from '../../../Core/Services/wishlist.service';
import { RecentlyViewedService } from '../../../Core/Services/recently-viewed.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Product } from '../../../Core/Models/product.model';
// import { CartHelperService } from '../../../Core/Services/';

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
    // private cartHelperService: CartHelperService,
    public authService: AuthService,
    private toastr: ToastrService
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
      error: (err) => {
        console.error('Error loading product:', err);
        this.isLoading = false;
        this.toastr.error(
          'Failed to load product details. Please try again.',
          'Error',
          {
            timeOut: 3000,
            progressBar: true
          }
        );
      }
    });
  }

  trackRecentlyViewed(productId: string) {
    this.recentlyViewedService.addRecentlyViewed(productId).subscribe({
      next: () => {
      },
      error: (err) => {
        console.error('Failed to track recently viewed:', err);
      }
    });
  }

  checkWishlistStatus(productId: string) {
    this.wishlistService.isInWishlist(productId).subscribe({
      next: (response) => {
        if (response.success) {
          this.isInWishlist = response.data;
        }
      },
      error: (err) => {
        console.error('Failed to check wishlist status:', err);
      }
    });
  }

  toggleWishlist() {
    if (!this.product) return;

    if (!this.authService.isAuthenticated()) {
      this.toastr.warning('Please login to manage your wishlist', 'Login Required');
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
      next: (response) => {
        if (response.success) {
          this.isInWishlist = true;
          this.toastr.success('Added to wishlist!', 'Success', {
            timeOut: 2000,
            progressBar: true
          });
        }
      },
      error: (err) => {
        console.error('Failed to add to wishlist:', err);
        this.toastr.error('Failed to add to wishlist', 'Error');
      }
    });
  }

  removeFromWishlist() {
    if (!this.product) return;

    this.wishlistService.removeFromWishlist(this.product.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.isInWishlist = false;
          this.toastr.success('Removed from wishlist!', 'Success', {
            timeOut: 2000,
            progressBar: true
          });
        }
      },
      error: (err) => {
        console.error('Failed to remove from wishlist:', err);
        this.toastr.error('Failed to remove from wishlist', 'Error');
      }
    });
  }

  increaseQuantity() {
    if (this.product && this.selectedQuantity < this.product.quantity) {
      this.selectedQuantity++;
    } 
    else if (this.product && this.selectedQuantity >= this.product.quantity) {
      this.toastr.warning(
        'Cannot exceed available stock',
        'Stock Limit',
        {
          timeOut: 2000,
          progressBar: true
        }
      );
    }
  }

  // addToCart() {
  //   if (!this.product) return;
  //   this.cartHelperService.addToCart(this.product, this.selectedQuantity);
  // }

  decreaseQuantity() {
    if (this.selectedQuantity > 1) {
      this.selectedQuantity--;
    } else {
      this.toastr.warning(
        'Minimum quantity is 1',
        'Limit',
        {
          timeOut: 2000,
          progressBar: true
        }
      );
    }
  }
}