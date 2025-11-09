using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Application.Services.Interfaces
{
    public interface ICategoryServices
    {
        Task<IEnumerable<GetCategory>> GetAllAsync();

        Task<GetCategory> GetAsync(Guid id);

        Task<ServiceResponse> AddAsync(CreateCategory product);

        Task<ServiceResponse> UpdateAsync(UpdateCategory product);

        Task<ServiceResponse> DeleteAsync(Guid id);
    }
}
