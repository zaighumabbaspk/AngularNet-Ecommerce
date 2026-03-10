using eCommerce.Domain.Entities;
using eCommerce.Domain.Interface;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace eCommerce.infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddOrUpdateCartItemAsync(string userId, Guid productId, int quantity)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        CartItems = new List<CartItem>()
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync(); // Save cart first
                }

                // Check if the item already exists in the cart
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity; // Update quantity
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    _context.CartItems.Add(cartItem); // Add new item
                }

                // Save all changes
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflict by reloading and retrying
                foreach (var entry in ex.Entries)
                {
                    await entry.ReloadAsync();
                }
                // Retry the operation
                return await _context.SaveChangesAsync();
            }
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }


        public async Task<int> ClearCartAsync(string userId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || cart.CartItems.Count == 0)
                    return 0;

                _context.CartItems.RemoveRange(cart.CartItems);
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflict by reloading and retrying
                foreach (var entry in ex.Entries)
                {
                    await entry.ReloadAsync();
                }
                return await _context.SaveChangesAsync();
            }
        }

        public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)  
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }


        public async Task<CartItem?> GetCartItemAsync(Guid cartItemId)
        {
             return await _context.CartItems.
                Include(ci => ci.Product).
                FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }

        public async Task<int> RemoveCartItemAsync(Guid cartItemId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

                if (cartItem == null)
                    return 0;

                _context.CartItems.Remove(cartItem);
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflict by reloading and retrying
                foreach (var entry in ex.Entries)
                {
                    await entry.ReloadAsync();
                }
                return await _context.SaveChangesAsync();
            }
        }


        public async Task<int> UpdateCartItemQuantityAsync(Guid cartItemId, int quantity)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

                if (cartItem == null)
                    return 0;

                if (quantity <= 0)
                {
                    // Remove item if quantity is 0 or negative
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                }

                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflict by reloading and retrying
                foreach (var entry in ex.Entries)
                {
                    await entry.ReloadAsync();
                }
                return await _context.SaveChangesAsync();
            }
        }
    }
}
