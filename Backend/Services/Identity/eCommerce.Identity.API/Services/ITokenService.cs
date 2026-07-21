using eCommerce.Identity.API.Models;

namespace eCommerce.Identity.API.Services;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
}
