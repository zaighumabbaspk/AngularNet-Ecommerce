using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Search;
using eCommerce.Application.Services;
using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;

namespace eCommerce.Application.Services.implementation
{
    public class SearchService : ISearchService
    {
        private readonly IProductRepository _productRepository;
        private readonly IGeneric<Category> _categoryRepository;
        private readonly ISearchAnalyticsRepository _searchAnalyticsRepository;
        private readonly IMapper _mapper;

        public SearchService(
            IProductRepository productRepository,
            IGeneric<Category> categoryRepository,
            ISearchAnalyticsRepository searchAnalyticsRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _searchAnalyticsRepository = searchAnalyticsRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<SearchResponse>> SearchProductsAsync(SearchRequest request)
        {
            try
            {
                // For now, use basic search functionality with existing repository methods
                var allProducts = await _productRepository.GetAllWithCategoryAsync();
                var query = allProducts.AsQueryable();

                // Text search - only apply if query is not empty
                if (!string.IsNullOrWhiteSpace(request.Query))
                {
                    var searchTerm = request.Query.ToLower().Trim();
                    query = query.Where(p => 
                        p.Name.ToLower().Contains(searchTerm) ||
                        p.Description.ToLower().Contains(searchTerm) ||
                        (!string.IsNullOrEmpty(p.Brand) && p.Brand.ToLower().Contains(searchTerm)) ||
                        p.Category.Name.ToLower().Contains(searchTerm));
                }

                // Price range filter
                if (request.MinPrice.HasValue)
                    query = query.Where(p => p.Price >= request.MinPrice.Value);
                
                if (request.MaxPrice.HasValue)
                    query = query.Where(p => p.Price <= request.MaxPrice.Value);

                // Category filter
                if (request.CategoryIds != null && request.CategoryIds.Any())
                    query = query.Where(p => request.CategoryIds.Contains(p.CategoryId));

                // Brand filter
                if (request.Brands != null && request.Brands.Any())
                    query = query.Where(p => !string.IsNullOrEmpty(p.Brand) && request.Brands.Contains(p.Brand));

                // Rating filter
                if (request.MinRating.HasValue)
                    query = query.Where(p => p.Rating >= request.MinRating.Value);

                // Stock filter
                if (request.InStock.HasValue && request.InStock.Value)
                    query = query.Where(p => p.Quantity > 0);

                // Get total count before pagination
                var totalCount = query.Count();

                // Sorting
                query = request.SortBy?.ToLower() switch
                {
                    "price_asc" => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "rating" => query.OrderByDescending(p => p.Rating),
                    "newest" => query.OrderByDescending(p => p.CreatedAt),
                    "popular" => query.OrderByDescending(p => p.ReviewCount),
                    _ => query.OrderBy(p => p.Name)
                };

                // Pagination
                var products = query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                // Map to DTOs
                var productResults = products.Select(p => new SearchProductResult
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.Image,
                    CategoryName = p.Category.Name,
                    Brand = p.Brand,
                    Rating = p.Rating,
                    ReviewCount = p.ReviewCount,
                    StockQuantity = p.Quantity,
                    IsInStock = p.Quantity > 0,
                    CreatedAt = p.CreatedAt
                }).ToList();

                // Get available filters
                var availableFilters = await GetAvailableFiltersAsync();

                var response = new SearchResponse
                {
                    Products = productResults,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                    CurrentPage = request.Page,
                    AvailableFilters = availableFilters
                };

                // Track search
                if (!string.IsNullOrEmpty(request.Query))
                {
                    await TrackSearchAsync(request.Query, totalCount);
                }

                return new ServiceResponse<SearchResponse>
                {
                    Success = true,
                    Message = "Search completed successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SearchResponse>
                {
                    Success = false,
                    Message = $"Error searching products: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<AutocompleteResponse>> GetAutocompleteAsync(AutocompleteRequest request)
        {
            try
            {
                var suggestions = new List<AutocompleteSuggestion>();
                var searchTerm = request.Query.ToLower();

                // Product suggestions
                var allProducts = await _productRepository.GetAllWithCategoryAsync();
                var products = allProducts
                    .Where(p => p.Name.ToLower().Contains(searchTerm))
                    .Take(request.Limit / 2)
                    .Select(p => new AutocompleteSuggestion
                    {
                        Text = p.Name,
                        Type = "product",
                        Id = p.Id,
                        ImageUrl = p.Image
                    })
                    .ToList();

                suggestions.AddRange(products);

                // Category suggestions
                var allCategories = await _categoryRepository.GetAllAsync();
                var categories = allCategories
                    .Where(c => c.Name.ToLower().Contains(searchTerm))
                    .Take(request.Limit / 4)
                    .Select(c => new AutocompleteSuggestion
                    {
                        Text = c.Name,
                        Type = "category",
                        Id = c.Id
                    })
                    .ToList();

                suggestions.AddRange(categories);

                // Brand suggestions
                var brands = allProducts
                    .Where(p => !string.IsNullOrEmpty(p.Brand) && p.Brand.ToLower().Contains(searchTerm))
                    .Select(p => p.Brand)
                    .Distinct()
                    .Take(request.Limit / 4)
                    .Select(b => new AutocompleteSuggestion
                    {
                        Text = b,
                        Type = "brand"
                    })
                    .ToList();

                suggestions.AddRange(brands);

                var response = new AutocompleteResponse
                {
                    Suggestions = suggestions.Take(request.Limit).ToList()
                };

                return new ServiceResponse<AutocompleteResponse>
                {
                    Success = true,
                    Message = "Autocomplete suggestions retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AutocompleteResponse>
                {
                    Success = false,
                    Message = $"Error getting autocomplete suggestions: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<List<string>>> GetPopularSearchTermsAsync(int limit = 10)
        {
            try
            {
                // For now, return empty list until we implement search analytics repository
                var popularTerms = new List<string>();

                return new ServiceResponse<List<string>>
                {
                    Success = true,
                    Message = "Popular search terms retrieved successfully",
                    Data = popularTerms
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<string>>
                {
                    Success = false,
                    Message = $"Error getting popular search terms: {ex.Message}"
                };
            }
        }

        public async Task TrackSearchAsync(string searchTerm, int resultCount, string? userId = null, string ipAddress = "")
        {
            try
            {
                // For now, just log the search - implement analytics later
                // This is a placeholder for search analytics tracking
            }
            catch (Exception)
            {
                // Silently fail for analytics tracking
            }
        }

        private async Task<SearchFilters> GetAvailableFiltersAsync()
        {
            var allProducts = await _productRepository.GetAllWithCategoryAsync();
            
            var minPrice = allProducts.Any() ? allProducts.Min(p => p.Price) : 0;
            var maxPrice = allProducts.Any() ? allProducts.Max(p => p.Price) : 0;
            
            var brands = allProducts
                .Where(p => !string.IsNullOrEmpty(p.Brand))
                .Select(p => p.Brand)
                .Distinct()
                .ToList();

            var allCategories = await _categoryRepository.GetAllAsync();
            var categories = allCategories.Select(c => new CategoryFilter
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = allProducts.Count(p => p.CategoryId == c.Id)
            }).ToList();

            return new SearchFilters
            {
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Brands = brands,
                Categories = categories
            };
        }
    }
}