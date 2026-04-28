using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.RecentlyViewed;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;

namespace eCommerce.Application.Services.implementation
{
    public class RecentlyViewedService : IRecentlyViewedService
    {
        private readonly IRecentlyViewedRepository _recentlyViewedRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public RecentlyViewedService(
            IRecentlyViewedRepository recentlyViewedRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _recentlyViewedRepository = recentlyViewedRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> AddRecentlyViewedAsync(AddRecentlyViewedRequest request)
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

                // Check if already exists for this user
                var existingItem = await _recentlyViewedRepository.GetRecentlyViewedItemAsync(request.UserId, request.ProductId);

                if (existingItem != null)
                {
                    // Update the viewed time
                    existingItem.ViewedAt = DateTime.UtcNow;
                    await _recentlyViewedRepository.UpdateAsync(existingItem);
                }
                else
                {
                    // Add new recently viewed item
                    var recentlyViewedItem = new RecentlyViewed
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        ProductId = request.ProductId,
                        ViewedAt = DateTime.UtcNow
                    };

                    await _recentlyViewedRepository.AddAsync(recentlyViewedItem);

                    // Keep only the last 20 items per user - implement this in repository later
                }

                return new ServiceResponse<string>
                {
                    Success = true,
                    Message = "Product added to recently viewed successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = $"Error adding product to recently viewed: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<RecentlyViewedResponse>> GetRecentlyViewedAsync(string userId, int limit = 10)
        {
            try
            {
                var recentlyViewedItems = await _recentlyViewedRepository.GetUserRecentlyViewedAsync(userId, limit);

                var items = new List<RecentlyViewedItem>();
                foreach (var rv in recentlyViewedItems)
                {
                    var product = await _productRepository.GetAsync(rv.ProductId);
                    if (product != null)
                    {
                        items.Add(new RecentlyViewedItem
                        {
                            ProductId = rv.ProductId,
                            ProductName = product.Name,
                            Price = product.Price,
                            ImageUrl = product.Image,
                            ViewedAt = rv.ViewedAt
                        });
                    }
                }

                var response = new RecentlyViewedResponse
                {
                    Items = items
                };

                return new ServiceResponse<RecentlyViewedResponse>
                {
                    Success = true,
                    Message = "Recently viewed products retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RecentlyViewedResponse>
                {
                    Success = false,
                    Message = $"Error retrieving recently viewed products: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<string>> ClearRecentlyViewedAsync(string userId)
        {
            try
            {
                await _recentlyViewedRepository.ClearUserRecentlyViewedAsync(userId);

                return new ServiceResponse<string>
                {
                    Success = true,
                    Message = "Recently viewed history cleared successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = $"Error clearing recently viewed history: {ex.Message}"
                };
            }
        }
    }
}