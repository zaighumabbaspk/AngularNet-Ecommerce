using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;
using eCommerce.infrastructure.Data;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .ToListAsync();
        }

        public async Task<Product?> GetWithCategoryByIdAsync(Guid id)
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
