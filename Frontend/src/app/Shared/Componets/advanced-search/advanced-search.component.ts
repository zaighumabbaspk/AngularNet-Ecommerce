import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
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
    private router: Router
  ) {}

  ngOnInit() {
    this.loadInitialData();
  }

  loadInitialData() {
    this.performSearch();
  }

  performSearch() {
    this.isLoading = true;
    
    this.searchService.searchProducts(this.searchRequest).subscribe({
      next: (response) => {
        if (response.success) {
          this.searchResponse = response.data;
          this.availableFilters = response.data.availableFilters;
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Search failed:', error);
        this.isLoading = false;
      }
    });
  }

  onSearchSubmit() {
    this.searchRequest.page = 1; // Reset to first page
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