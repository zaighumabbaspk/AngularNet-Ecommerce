namespace eCommerce.Application.DTOs.Search
{
    public class SearchRequest
    {
        public string? Query { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public List<string>? Brands { get; set; }
        public decimal? MinRating { get; set; }
        public bool? InStock { get; set; }
        public string? SortBy { get; set; } // price_asc, price_desc, rating, newest, popular
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class SearchResponse
    {
        public List<SearchProductResult> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public SearchFilters AvailableFilters { get; set; } = new();
    }

    public class SearchProductResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public int StockQuantity { get; set; }
        public bool IsInStock { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SearchFilters
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<string> Brands { get; set; } = new();
        public List<CategoryFilter> Categories { get; set; } = new();
    }

    public class CategoryFilter
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }

    public class AutocompleteRequest
    {
        public string Query { get; set; } = string.Empty;
        public int Limit { get; set; } = 10;
    }

    public class AutocompleteResponse
    {
        public List<AutocompleteSuggestion> Suggestions { get; set; } = new();
    }

    public class AutocompleteSuggestion
    {
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // product, category, brand
        public Guid? Id { get; set; }
        public string? ImageUrl { get; set; }
    }
}