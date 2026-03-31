using eCommerce.Application.DTOs.Search;
using eCommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ISearchService searchService, ILogger<SearchController> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        [HttpPost("products")]
        public async Task<IActionResult> SearchProducts([FromBody] SearchRequest request)
        {
            try
            {
                _logger.LogInformation("Searching products with query: {Query}", request.Query);
                var result = await _searchService.SearchProductsAsync(request);
                
                return Ok(new { success = result.Success, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> GetAutocomplete([FromQuery] string query, [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { success = false, message = "Query parameter is required" });
                }

                _logger.LogInformation("Getting autocomplete suggestions for: {Query}", query);
                var request = new AutocompleteRequest { Query = query, Limit = limit };
                var result = await _searchService.GetAutocompleteAsync(request);
                
                return Ok(new { success = result.Success, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting autocomplete suggestions");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("popular-terms")]
        public async Task<IActionResult> GetPopularSearchTerms([FromQuery] int limit = 10)
        {
            try
            {
                var result = await _searchService.GetPopularSearchTermsAsync(limit);
                
                return Ok(new { success = result.Success, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular search terms");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackSearch([FromBody] TrackSearchRequest request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                await _searchService.TrackSearchAsync(request.SearchTerm, request.ResultCount, request.UserId, ipAddress);
                
                return Ok(new { success = true, message = "Search tracked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking search");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }

    public class TrackSearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int ResultCount { get; set; }
        public string? UserId { get; set; }
    }
}