import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { WishlistService } from '../../../Core/Services/wishlist.service';
import { WishlistItem } from '../../../Core/Models/wishlist.models';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './wishlist.component.html',
  styleUrls: ['./wishlist.component.css']
})
export class WishlistComponent implements OnInit {
  wishlistItems: WishlistItem[] = [];
  isLoading = false;

  constructor(
    private wishlistService: WishlistService,
    public router: Router
  ) {}

  ngOnInit() {
    this.loadWishlist();
  }

  loadWishlist() {
    this.isLoading = true;
    
    this.wishlistService.getWishlist().subscribe({
      next: (response) => {
        console.log('Wishlist loaded:', response);
        if (response.success) {
          this.wishlistItems = response.data.items;
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load wishlist:', error);
        this.isLoading = false;
      }
    });
  }

  removeFromWishlist(productId: string) {
    this.wishlistService.removeFromWishlist(productId).subscribe({
      next: (response) => {
        if (response.success) {
          this.wishlistItems = this.wishlistItems.filter(item => item.productId !== productId);
        }
      },
      error: (error) => {
        console.error('Failed to remove from wishlist:', error);
      }
    });
  }

  viewProduct(productId: string) {
    this.router.navigate(['/product-detail', productId]);
  }
}