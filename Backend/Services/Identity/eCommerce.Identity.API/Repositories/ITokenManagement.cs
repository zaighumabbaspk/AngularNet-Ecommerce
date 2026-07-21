using System.Security.Claims;

namespace eCommerce.Identity.API.Repositories;

public interface ITokenManagement
{
    string GetRefreshToken();
    Task<int> AddRefreshToken(string userId, string refreshToken);
    Task<int> UpdateRefreshToken(string userId, string refreshToken);
    Task<bool> ValidateRefreshToken(string refreshToken);
    Task<string> GetUserIdByRefreshToken(string refreshToken);
}
