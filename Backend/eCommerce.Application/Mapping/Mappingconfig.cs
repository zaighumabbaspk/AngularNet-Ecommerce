using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Category;
using eCommerce.Application.DTOs.Product;
using eCommerce.Domain.Entities;
using eCommerce.Domain.Entities.Identity;
using eCommerceApp.Application.DTOs.Identity;
using Microsoft.AspNetCore.Builder;

public class Mappingconfig : Profile
{
    public Mappingconfig()
    {
        // Category mappings
        CreateMap<CreateCategory, Category>().ReverseMap();
        CreateMap<UpdateCategory, Category>().ReverseMap();
        CreateMap<Category, GetCategory>();

        // Product mappings
        CreateMap<CreateProduct, Product>().ReverseMap();
        CreateMap<UpdateProduct, Product>().ReverseMap();

        // No recursion — just map fields
        CreateMap<Product, GetProduct>();
        CreateMap<GetProduct, Product>();

        //CreateMap<CreateUser, AppUser>();
        //CreateMap<LoginUser, AppUser>();

        CreateMap<CreateUser, AppUser>();
        CreateMap<LoginUser, AppUser>();

    }
}
