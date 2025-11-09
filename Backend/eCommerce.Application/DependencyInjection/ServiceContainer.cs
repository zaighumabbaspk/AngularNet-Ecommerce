using AutoMapper;
using eCommerce.Application.Services.implementation;
using eCommerce.Application.Services.implementation.Authentication;
using eCommerce.Application.Services.Implementation;

using eCommerce.Application.Services.Interfaces;

using eCommerce.Application.Validations.Authenticaton;
using eCommerceApp.Application.Validations;
using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Mappingconfig));

            services.AddScoped<ICategoryServices, CategoryService>();
            services.AddScoped<IProductServices, ProductService>();
             services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
          
            services.AddScoped<IValidationService, ValidationService>();

            return services;
        }
    }
}
