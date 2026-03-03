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

        CreateMap<CreateUser, AppUser>();
        CreateMap<LoginUser, AppUser>();

        // Cart mappings
        CreateMap<Cart, GetCart>()
            .ForMember(dest => dest.Total, opt => opt.Ignore())
            .ForMember(dest => dest.TotalItems, opt => opt.Ignore());

        CreateMap<CartItem, GetCartItem>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.Image))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Quantity * src.Product.Price))
            .ForMember(dest => dest.AvailableStock, opt => opt.MapFrom(src => src.Product.Quantity));


    }
}
