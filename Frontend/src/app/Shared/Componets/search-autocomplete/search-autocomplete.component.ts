import { Component, OnInit, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';
import { SearchService } from '../../../Core/Services/search.service';
import { AutocompleteSuggestion } from '../../../Core/Models/search.models';

@Component({
  selector: 'app-search-autocomplete',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-autocomplete.component.html',
  styleUrls: ['./search-autocomplete.component.css']
})
export class SearchAutocompleteComponent implements OnInit, OnDestroy {
  @ViewChild('searchInput', { static: true }) searchInput!: ElementRef;
  
  searchQuery = '';
  suggestions: AutocompleteSuggestion[] = [];
  showSuggestions = false;
  isLoading = false;
  
  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(
    private searchService: SearchService,
    private router: Router
  ) {}

  ngOnInit() {
    this.setupAutocomplete();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  setupAutocomplete() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(query => {
        if (query.length < 2) {
          return of({ success: true, data: { suggestions: [] } });
        }
        
        this.isLoading = true;
        return this.searchService.getAutocomplete(query, 8);
      })
    ).subscribe({
      next: (response) => {
        this.isLoading = false;
        console.log('Autocomplete response:', response);
        if (response.success && response.data) {
          this.suggestions = response.data.suggestions || [];
          this.showSuggestions = this.suggestions.length > 0;
        } else {
          this.suggestions = [];
          this.showSuggestions = false;
        }
      },
      error: (error) => {
        console.error('Autocomplete failed:', error);
        this.isLoading = false;
        this.suggestions = [];
        this.showSuggestions = false;
      }
    });
  }

  onSearchInput() {
    const trimmedQuery = this.searchQuery.trim();
    if (trimmedQuery.length >= 2) {
      this.searchSubject.next(trimmedQuery);
    } else {
      this.suggestions = [];
      this.showSuggestions = false;
      this.isLoading = false;
    }
  }

  onSearchSubmit() {
    if (this.searchQuery.trim()) {
      this.hideSuggestions();
      this.router.navigate(['/search'], { 
        queryParams: { q: this.searchQuery.trim() } 
      });
    }
  }

  selectSuggestion(suggestion: AutocompleteSuggestion) {
    this.searchQuery = suggestion.text;
    
    // Try different property names for the ID
    const productId = suggestion.id || (suggestion as any).productId || (suggestion as any).itemId;
    
    if (suggestion.type === 'product' && productId) {
      this.hideSuggestions();
      this.router.navigate(['/product-detail', productId]);
    } else {
      this.hideSuggestions();
      this.router.navigate(['/search'], { 
        queryParams: { q: suggestion.text } 
      });
    }
  }

  onFocus() {
    if (this.suggestions.length > 0) {
      this.showSuggestions = true;
    }
  }

  onBlur() {
    // Delay hiding to allow click on suggestions
    setTimeout(() => {
      this.hideSuggestions();
    }, 300);
  }

  hideSuggestions() {
    this.showSuggestions = false;
  }

  getSuggestionIcon(type: string): string {
    switch (type) {
      case 'product':
        return 'fas fa-box';
      case 'category':
        return 'fas fa-tags';
      case 'brand':
        return 'fas fa-trademark';
      default:
        return 'fas fa-search';
    }
  }
}