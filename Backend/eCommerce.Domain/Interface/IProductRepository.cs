using eCommerce.Domain.Entities;

namespace eCommerce.Domain.Interface
{
    public interface IProductRepository : IGeneric<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        Task<Product?> GetWithCategoryByIdAsync(Guid id);
    }
}
