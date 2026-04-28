
using eCommerce.Domain.Entities.Identity;
using eCommerce.Domain.Services.Interfaces.Authentication;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eCommerce.infrastructure.Repositories.Authentication
{
    public class TokenManagement : ITokenManagement
    {

        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public TokenManagement(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
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


        public string GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public  string GetRefreshToken()
        {
            const int byteSize = 64;
            Byte[] randomByte = new Byte[byteSize];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomByte);
               
            }
            return Convert.ToBase64String(randomByte);

        }

        public List<Claim> GetUserClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            if(jwtToken != null)
            {
                return jwtToken.Claims.ToList();
            }
            return [];

        }

        public async Task<string> GetUserIdByRefreshToken(string refreshToken) =>   
            (await _context.RefreshToken.FirstOrDefaultAsync(rt => rt.Token == refreshToken))?.UserId!;


        public async Task<int> UpdateRefreshToken(string userId, string refreshToken)
        {
       
            var existingToken = await _context.RefreshToken
                .FirstOrDefaultAsync(rt => rt.UserId == userId);

            if (existingToken == null)
            {
                _context.RefreshToken.Add(new RefreshToken
                {
                    UserId = userId,
                    Token = refreshToken
                });
            }
            else
            {
             
                existingToken.Token = refreshToken;
            
            }

            return await _context.SaveChangesAsync();
        }


        public async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            var user = await _context.RefreshToken
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            return user != null;
        }
    }



}
