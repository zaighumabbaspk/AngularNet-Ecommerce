using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Cart;
using eCommerce.Application.DTOs.Category;
using eCommerce.Application.DTOs.Order;
using eCommerce.Application.DTOs.Product;
using eCommerce.Domain.Entities;
using eCommerce.Domain.Entities.Identity;
using eCommerceApp.Application.DTOs.Identity;

public class Mappingconfig : Profile
{
    public Mappingconfig()
    {
        // ===== CATEGORY MAPPINGS =====
        CreateMap<CreateCategory, Category>().ReverseMap();
        CreateMap<UpdateCategory, Category>().ReverseMap();
        CreateMap<Category, GetCategory>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

        // ===== PRODUCT MAPPINGS =====
        CreateMap<CreateProduct, Product>().ReverseMap();
        CreateMap<UpdateProduct, Product>().ReverseMap();
        CreateMap<Product, GetProduct>();

        // Reverse mapping for GetProduct → Product (for updates if needed)
        CreateMap<GetProduct, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore());

        // ===== IDENTITY/AUTHENTICATION MAPPINGS =====
        CreateMap<CreateUser, AppUser>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<LoginUser, AppUser>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<AppUser, LoginResponse>()
            .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.Id));

        // ===== CART MAPPINGS =====
        CreateMap<Cart, GetCart>()
            .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => 
                src.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => 
                src.CartItems.Sum(ci => ci.Quantity)));

        CreateMap<CartItem, GetCartItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.Image))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Quantity * src.Product.Price))
            .ForMember(dest => dest.AvailableStock, opt => opt.MapFrom(src => src.Product.Quantity));

        // Reverse mapping for cart requests
        CreateMap<AddToCartRequest, CartItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

        CreateMap<UpdateCartItemRequest, CartItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CartItemId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

        // ===== ORDER MAPPINGS =====
        CreateMap<Order, GetOrder>()
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.StatusHistory, opt => opt.MapFrom(src => src.StatusHistory));

        CreateMap<OrderItem, GetOrderItem>();

        CreateMap<OrderStatusHistory, GetOrderStatusHistory>();

        CreateMap<CreateOrder, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.StatusHistory, opt => opt.Ignore());

        CreateMap<CreateOrderItem, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.UnitPrice * src.Quantity))
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.ProductName, opt => opt.Ignore())
            .ForMember(dest => dest.ProductDescription, opt => opt.Ignore())
            .ForMember(dest => dest.ProductImage, opt => opt.Ignore());
    }
}