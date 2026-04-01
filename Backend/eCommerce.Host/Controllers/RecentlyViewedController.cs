using eCommerce.Application.DTOs.RecentlyViewed;
using eCommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecentlyViewedController : ControllerBase
    {
        private readonly IRecentlyViewedService _recentlyViewedService;

        public RecentlyViewedController(IRecentlyViewedService recentlyViewedService)
        {
            _recentlyViewedService = recentlyViewedService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRecentlyViewed([FromBody] AddRecentlyViewedRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            request.UserId = userId;
            var result = await _recentlyViewedService.AddRecentlyViewedAsync(request);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentlyViewed([FromQuery] int limit = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var result = await _recentlyViewedService.GetRecentlyViewedAsync(userId, limit);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearRecentlyViewed()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var result = await _recentlyViewedService.ClearRecentlyViewedAsync(userId);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }
    }
}