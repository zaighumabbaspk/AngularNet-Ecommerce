using eCommerce.Identity.API.Data;
using eCommerce.Identity.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace eCommerce.Identity.API.Repositories;

public class TokenManagement : ITokenManagement
{
    private readonly IdentityDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenManagement> _logger;

    public TokenManagement(IdentityDbContext context, IConfiguration configuration, ILogger<TokenManagement> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<int> AddRefreshToken(string userId, string refreshToken)
    {
        _context.RefreshToken.Add(new RefreshToken
        {
            UserId = userId,
            Token = refreshToken,
        });
        return await _context.SaveChangesAsync();
    }

    public string GetRefreshToken()
    {
        const int byteSize = 64;
        var randomByte = new byte[byteSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomByte);
        return Convert.ToBase64String(randomByte);
    }

    public async Task<int> UpdateRefreshToken(string userId, string refreshToken)
    {
        var existing = await _context.RefreshToken.FirstOrDefaultAsync(r => r.UserId == userId);
        if (existing == null)
        {
            _context.RefreshToken.Add(new RefreshToken { UserId = userId, Token = refreshToken });
        }
        else
        {
            existing.Token = refreshToken;
        }

        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed saving refresh token for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ValidateRefreshToken(string refreshToken)
    {
        var token = await _context.RefreshToken.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        return token != null;
    }

    public async Task<string> GetUserIdByRefreshToken(string refreshToken)
        => (await _context.RefreshToken.FirstOrDefaultAsync(rt => rt.Token == refreshToken))?.UserId ?? string.Empty;
}
