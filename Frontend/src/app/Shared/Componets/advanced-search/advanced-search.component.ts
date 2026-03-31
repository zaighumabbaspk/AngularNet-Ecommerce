import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { SearchService } from '../../../Core/Services/search.service';
import { SearchRequest, SearchResponse, SearchProductResult, SearchFilters } from '../../../Core/Models/search.models';

@Component({
  selector: 'app-advanced-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './advanced-search.component.html',
  styleUrls: ['./advanced-search.component.css']
})
export class AdvancedSearchComponent implements OnInit {
  searchRequest: SearchRequest = {
    query: '',
    page: 1,
    pageSize: 12,
    sortBy: 'name'
  };

  searchResponse: SearchResponse | null = null;
  availableFilters: SearchFilters | null = null;
  isLoading = false;
  showFilters = false;

  constructor(
    private searchService: SearchService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // Get query parameter from URL
    this.route.queryParams.subscribe(params => {
      if (params['q']) {
        this.searchRequest.query = params['q'];
      }
      this.loadInitialData();
    });
  }

  loadInitialData() {
    this.performSearch();
  }

  performSearch() {
    this.isLoading = true;
    
    this.searchService.searchProducts(this.searchRequest).subscribe({
      next: (response) => {
        console.log('Search response:', response);
        if (response.success && response.data) {
          this.searchResponse = response.data;
          this.availableFilters = response.data.availableFilters;
          
          // Track search if there's a query
          if (this.searchRequest.query && this.searchRequest.query.trim()) {
            this.trackSearch(this.searchRequest.query, response.data.totalCount);
          }
        } else {
          console.error('Search failed:', response.message);
          this.searchResponse = null;
          this.availableFilters = null;
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Search failed:', error);
        this.searchResponse = null;
        this.availableFilters = null;
        this.isLoading = false;
      }
    });
  }

  private trackSearch(searchTerm: string, resultCount: number) {
    // Get user ID if available (you might need to inject AuthService)
    this.searchService.trackSearch(searchTerm, resultCount).subscribe({
      next: () => console.log('Search tracked successfully'),
      error: (error) => console.error('Failed to track search:', error)
    });
  }

  onSearchSubmit() {
    this.searchRequest.page = 1; // Reset to first page
    // Update URL with search query
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { q: this.searchRequest.query },
      queryParamsHandling: 'merge'
    });
    this.performSearch();
  }

  onFilterChange() {
    this.searchRequest.page = 1; // Reset to first page when filters change
    this.performSearch();
  }

  onSortChange() {
    this.searchRequest.page = 1; // Reset to first page when sort changes
    this.performSearch();
  }

  onPageChange(page: number) {
    this.searchRequest.page = page;
    this.performSearch();
  }

  onCategoryChange(categoryId: string, event: any) {
    if (!this.searchRequest.categoryIds) {
      this.searchRequest.categoryIds = [];
    }
    
    if (event.target.checked) {
      this.searchRequest.categoryIds.push(categoryId);
    } else {
      this.searchRequest.categoryIds = this.searchRequest.categoryIds.filter(id => id !== categoryId);
    }
    
    this.onFilterChange();
  }

  onBrandChange(brand: string, event: any) {
    if (!this.searchRequest.brands) {
      this.searchRequest.brands = [];
    }
    
    if (event.target.checked) {
      this.searchRequest.brands.push(brand);
    } else {
      this.searchRequest.brands = this.searchRequest.brands.filter(b => b !== brand);
    }
    
    this.onFilterChange();
  }

  toggleFilters() {
    this.showFilters = !this.showFilters;
  }

  clearFilters() {
    this.searchRequest = {
      query: this.searchRequest.query,
      page: 1,
      pageSize: 12,
      sortBy: 'name'
    };
    this.performSearch();
  }

  viewProduct(productId: string) {
    this.router.navigate(['/product-detail', productId]);
  }

  getPaginationPages(): number[] {
    if (!this.searchResponse) return [];
    
    const totalPages = this.searchResponse.totalPages;
    const currentPage = this.searchResponse.currentPage;
    const pages: number[] = [];
    
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }
}