using eCommerce.Application.DTOs.Search;
using eCommerce.Application.DTOs;

namespace eCommerce.Application.Services
{
    public interface ISearchService
    {
        Task<ServiceResponse<SearchResponse>> SearchProductsAsync(SearchRequest request);
        Task<ServiceResponse<AutocompleteResponse>> GetAutocompleteAsync(AutocompleteRequest request);
        Task<ServiceResponse<List<string>>> GetPopularSearchTermsAsync(int limit = 10);
        Task TrackSearchAsync(string searchTerm, int resultCount, string? userId = null, string ipAddress = "");
    }
}