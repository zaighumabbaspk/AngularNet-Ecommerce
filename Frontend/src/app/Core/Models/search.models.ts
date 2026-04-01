export interface SearchRequest {
  query?: string;
  minPrice?: number;
  maxPrice?: number;
  categoryIds?: string[];
  brands?: string[];
  minRating?: number;
  inStock?: boolean;
  sortBy?: string; // price_asc, price_desc, rating, newest, popular
  page: number;
  pageSize: number;
}

export interface SearchResponse {
  products: SearchProductResult[];
  totalCount: number;
  totalPages: number;
  currentPage: number;
  availableFilters: SearchFilters;
}

export interface SearchProductResult {
  id: string;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  categoryName: string;
  brand: string;
  rating: number;
  reviewCount: number;
  stockQuantity: number;
  isInStock: boolean;
  createdAt: string;
}

export interface SearchFilters {
  minPrice: number;
  maxPrice: number;
  brands: string[];
  categories: CategoryFilter[];
}

export interface CategoryFilter {
  id: string;
  name: string;
  productCount: number;
}

export interface AutocompleteRequest {
  query: string;
  limit: number;
}

export interface AutocompleteResponse {
  suggestions: AutocompleteSuggestion[];
}

export interface AutocompleteSuggestion {
  text: string;
  type: string; // product, category, brand
  id?: string;
  imageUrl?: string;
}