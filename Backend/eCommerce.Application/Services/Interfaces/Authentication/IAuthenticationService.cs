using eCommerce.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;


public interface IAuthenticationService
{
    Task<ServiceResponse> CreateUser(CreateUser user);
    Task<LoginResponse> LoginUser(LoginUser user);
    Task<LoginResponse> ReviveToken(string refreshToken);
}

