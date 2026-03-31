import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RecentlyViewedService } from '../../../Core/Services/recently-viewed.service';
import { RecentlyViewedItem } from '../../../Core/Models/recently-viewed.models';

@Component({
  selector: 'app-recently-viewed',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recently-viewed.component.html',
  styleUrls: ['./recently-viewed.component.css']
})
export class RecentlyViewedComponent implements OnInit {
  @Input() limit: number = 10;
  @Input() showHeader: boolean = true;
  
  recentlyViewedItems: RecentlyViewedItem[] = [];
  isLoading = false;

  constructor(
    private recentlyViewedService: RecentlyViewedService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadRecentlyViewed();
  }

  loadRecentlyViewed() {
    this.isLoading = true;
    
    this.recentlyViewedService.getRecentlyViewed(this.limit).subscribe({
      next: (response) => {
        if (response.success) {
          this.recentlyViewedItems = response.data.items;
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load recently viewed:', error);
        this.isLoading = false;
      }
    });
  }

  clearRecentlyViewed() {
    this.recentlyViewedService.clearRecentlyViewed().subscribe({
      next: (response) => {
        if (response.success) {
          this.recentlyViewedItems = [];
        }
      },
      error: (error) => {
        console.error('Failed to clear recently viewed:', error);
      }
    });
  }

  viewProduct(productId: string) {
    this.router.navigate(['/product-detail', productId]);
  }
}