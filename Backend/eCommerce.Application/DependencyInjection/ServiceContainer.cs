using AutoMapper;
using eCommerce.Application.DTOs.Email;
using eCommerce.Application.Services.Implementation;
using eCommerce.Application.Services.implementation;
using eCommerce.Application.Services.implementation.Authentication;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Application.Services.Interfaces.Authentication;
using eCommerce.Application.Validations.Authenticaton;
using eCommerceApp.Application.Validations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(Mappingconfig));

            services.AddScoped<ICategoryServices, CategoryService>();
            services.AddScoped<IProductServices, ProductService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICheckoutService, CheckoutService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IRecentlyViewedService, RecentlyViewedService>();

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
          
            services.AddScoped<IValidationService, ValidationService>();
            services.AddMvc();  
            return services;
        }
    }
}