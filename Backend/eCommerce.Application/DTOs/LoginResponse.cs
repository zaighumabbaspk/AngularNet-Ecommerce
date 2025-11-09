

namespace eCommerce.Application.DTOs
{
    public record LoginResponse
    (
        bool success = false,
        string Message = null!,
        string Token = null!,
        string RefreshToken = null!
        );

        
    
}
