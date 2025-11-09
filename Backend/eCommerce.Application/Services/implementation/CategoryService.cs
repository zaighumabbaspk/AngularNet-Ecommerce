using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Category;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Application.Services.Implementation
{
    public class CategoryService : ICategoryServices
    {
        private readonly IGeneric<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IGeneric<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse> AddAsync(CreateCategory category)
        {
            var mappedData = _mapper.Map<Category>(category);
            int result = await _categoryRepository.AddAsync(mappedData);

            return result > 0
                ? new ServiceResponse(true, "Category added successfully")
                : new ServiceResponse(false, "Failed to add category");
        }

        public async Task<ServiceResponse> DeleteAsync(Guid id)
        {
            int result = await _categoryRepository.DeleteAsync(id);

            return result > 0
                ? new ServiceResponse(true, "Category deleted successfully")
                : new ServiceResponse(false, "Failed to delete category");
        }

        public async Task<IEnumerable<GetCategory>> GetAllAsync()
        {
            // Fetch all categories including their products
            var rawData = await _categoryRepository.GetAllWithIncludesAsync(c => c.Products);

            if (rawData == null || !rawData.Any())
                return Enumerable.Empty<GetCategory>();

            var mappedData = _mapper.Map<IEnumerable<GetCategory>>(rawData);
            return mappedData;
        }

        public async Task<GetCategory> GetAsync(Guid id)
        {
            // Fetch single category including products
            var rawData = await _categoryRepository.GetAsyncWithIncludesAsync(id, c => c.Products);

            if (rawData == null)
                return null;

            var mappedData = _mapper.Map<GetCategory>(rawData);
            return mappedData;
        }

        public async Task<ServiceResponse> UpdateAsync(UpdateCategory category)
        {
            var mappedData = _mapper.Map<Category>(category);
            int result = await _categoryRepository.UpdateAsync(mappedData);

            return result > 0
                ? new ServiceResponse(true, "Category updated successfully")
                : new ServiceResponse(false, "Failed to update category");
        }
    }
}
