using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Product;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Application.Services.implementation
{
    public class ProductService : IProductServices
    {
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<ServiceResponse> AddAsync(CreateProduct product)
        {
            var mappedData = _mapper.Map<Product>(product);
            int result = await _productRepo.AddAsync(mappedData);

            return result > 0
              ? new ServiceResponse(true, "Product added successfully")
              : new ServiceResponse(false, "Failed to add product");
        }

        public async Task<ServiceResponse> DeleteAsync(Guid id)
        {
            int result = await _productRepo.DeleteAsync(id);
            return result > 0
                ? new ServiceResponse(true, "Product deleted successfully")
                : new ServiceResponse(false, "Failed to delete product");
        }

        public async Task<IEnumerable<GetProduct>> GetAllAsync()
        {
            var rawData = await _productRepo.GetAllWithCategoryAsync();

            if (rawData == null || !rawData.Any())
                return Enumerable.Empty<GetProduct>();

            var mappedData = _mapper.Map<IEnumerable<GetProduct>>(rawData);
            return mappedData;
        }

        public async Task<GetProduct> GetAsync(Guid id)
        {
            var rawData = await _productRepo.GetWithCategoryByIdAsync(id);

            if (rawData == null)
                return null;

            var mappedData = _mapper.Map<GetProduct>(rawData);
            return mappedData;
        }

        public async Task<ServiceResponse> UpdateAsync(UpdateProduct product)
        {
            var mappedData = _mapper.Map<Product>(product);
            int result = await _productRepo.UpdateAsync(mappedData);

            return result > 0
                ? new ServiceResponse(true, "Product updated successfully")
                : new ServiceResponse(false, "Failed to update product");
        }
    }
}
