using eCommerce.Application.DTOs;
using eCommerce.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Application.Services.Interfaces
{
    public interface IProductServices
    {
        Task<IEnumerable<GetProduct>> GetAllAsync();
        Task<GetProduct> GetAsync(Guid id);

        Task<ServiceResponse> AddAsync(CreateProduct product);

        Task<ServiceResponse> UpdateAsync(UpdateProduct product);

        Task<ServiceResponse> DeleteAsync(Guid id);
    }

}


