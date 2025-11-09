using eCommerceApp.Application.DTOs.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase

    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("create")]

        public async Task<IActionResult> CreateUser(CreateUser user)
        {
            var result = await _authService.CreateUser(user);
            return result.Success ? Ok(result) : BadRequest(result);

        }

        [HttpPost("login")]

        public async Task<IActionResult> LoginUser(LoginUser user)
        {
            var result = await _authService.LoginUser(user);
            return result.success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("refreshToken/{refreshToken}")]


        public async Task<IActionResult> ReviveToken( string refreshToken)
        {
            var result = await _authService.ReviveToken(refreshToken);
            return result.success ? Ok(result) : BadRequest(result);
        }

    }

}
