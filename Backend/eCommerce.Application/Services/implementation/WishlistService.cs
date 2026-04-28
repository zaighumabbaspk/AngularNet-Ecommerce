using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Wishlist;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;

namespace eCommerce.Application.Services.implementation
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> AddToWishlistAsync(AddToWishlistRequest request)
        {
            try
            {
                // Check if product exists
                var product = await _productRepository.GetAsync(request.ProductId);
                if (product == null)
                {
                    return new ServiceResponse<string>
                    {
                        Success = false,
                        Message = "Product not found"
                    };
                }

                // Check if already in wishlist
                var existingItem = await _wishlistRepository.GetWishlistItemAsync(request.UserId, request.ProductId);

                if (existingItem != null)
                {
                    return new ServiceResponse<string>
                    {
                        Success = false,
                        Message = "Product is already in your wishlist"
                    };
                }

                // Add to wishlist
                var wishlistItem = new Wishlist
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    CreatedAt = DateTime.UtcNow
                };

                await _wishlistRepository.AddAsync(wishlistItem);

                return new ServiceResponse<string>
                {
                    Success = true,
                    Message = "Product added to wishlist successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = $"Error adding product to wishlist: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<string>> RemoveFromWishlistAsync(Guid productId, string userId)
        {
            try
            {
                var wishlistItem = await _wishlistRepository.GetWishlistItemAsync(userId, productId);

                if (wishlistItem == null)
                {
                    return new ServiceResponse<string>
                    {
                        Success = false,
                        Message = "Product not found in wishlist"
                    };
                }

                await _wishlistRepository.DeleteAsync(wishlistItem.Id);

                return new ServiceResponse<string>
                {
                    Success = true,
                    Message = "Product removed from wishlist successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = $"Error removing product from wishlist: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<WishlistResponse>> GetUserWishlistAsync(string userId)
        {
            try
            {
                var wishlistItems = await _wishlistRepository.GetUserWishlistAsync(userId);

                var items = new List<WishlistItem>();
                foreach (var w in wishlistItems)
                {
                    var product = await _productRepository.GetWithCategoryByIdAsync(w.ProductId);
                    if (product != null)
                    {
                        items.Add(new WishlistItem
                        {
                            Id = w.Id,
                            ProductId = w.ProductId,
                            ProductName = product.Name,
                            ProductDescription = product.Description,
                            Price = product.Price,
                            ImageUrl = product.Image,
                            CategoryName = product.Category.Name,
                            IsInStock = product.Quantity > 0,
                            AddedAt = w.CreatedAt
                        });
                    }
                }

                var response = new WishlistResponse
                {
                    Items = items.OrderByDescending(i => i.AddedAt).ToList(),
                    TotalCount = items.Count
                };

                return new ServiceResponse<WishlistResponse>
                {
                    Success = true,
                    Message = "Wishlist retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<WishlistResponse>
                {
                    Success = false,
                    Message = $"Error retrieving wishlist: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> IsInWishlistAsync(Guid productId, string userId)
        {
            try
            {
                var exists = await _wishlistRepository.IsInWishlistAsync(userId, productId);

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Wishlist status checked successfully",
                    Data = exists
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error checking wishlist status: {ex.Message}"
                };
            }
        }
    }
}