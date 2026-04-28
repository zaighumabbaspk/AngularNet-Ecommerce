using eCommerce.Application.DTOs.Search;
using eCommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost("products")]
        public async Task<IActionResult> SearchProducts([FromBody] SearchRequest request)
        {
            var result = await _searchService.SearchProductsAsync(request);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> GetAutocomplete([FromQuery] string query, [FromQuery] int limit = 10)
        {
            var request = new AutocompleteRequest { Query = query, Limit = limit };
            var result = await _searchService.GetAutocompleteAsync(request);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("popular-terms")]
        public async Task<IActionResult> GetPopularSearchTerms([FromQuery] int limit = 10)
        {
            var result = await _searchService.GetPopularSearchTermsAsync(limit);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackSearch([FromBody] TrackSearchRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            await _searchService.TrackSearchAsync(request.SearchTerm, request.ResultCount, request.UserId, ipAddress);
            
            return Ok(new { message = "Search tracked successfully" });
        }
    }

    public class TrackSearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int ResultCount { get; set; }
        public string? UserId { get; set; }
    }
}