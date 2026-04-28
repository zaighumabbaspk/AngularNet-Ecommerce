using eCommerce.Domain.Entities;
using eCommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace eCommerce.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

        // New entities for advanced search
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<RecentlyViewed> RecentlyViewed { get; set; }
        public DbSet<SearchAnalytics> SearchAnalytics { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User",
                    NormalizedName = "USER"
                }
            );
            builder.Entity<Cart>()
                .HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<Cart>(c => c.UserId);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
               .HasOne(ci => ci.Product)
               .WithMany()
               .HasForeignKey(ci => ci.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Cart>()
              .HasIndex(c => c.UserId)
              .IsUnique();

            // Order to User relationship
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .Property(o => o.GuestEmail)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Entity<Order>()
                .Property(o => o.GuestOrderToken)
                .HasMaxLength(100)
                .IsRequired(false);

            // Order to OrderItems relationship
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem to Product relationship
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order to StatusHistory relationship
            builder.Entity<OrderStatusHistory>()
                .ToTable("OrderStatusHistories") // Explicitly set table name
                .HasOne(osh => osh.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(osh => osh.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Wishlist relationships
            builder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wishlist>()
                .HasOne(w => w.Product)
                .WithMany(p => p.WishlistItems)
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Recently Viewed relationships
            builder.Entity<RecentlyViewed>()
                .HasOne(rv => rv.User)
                .WithMany()
                .HasForeignKey(rv => rv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RecentlyViewed>()
                .HasOne(rv => rv.Product)
                .WithMany(p => p.RecentlyViewedItems)
                .HasForeignKey(rv => rv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.Entity<Order>()
                .HasIndex(o => o.UserId);

            builder.Entity<Order>()
                .HasIndex(o => o.CreatedAt);

            builder.Entity<Order>()
                .HasIndex(o => o.Status);

            // Search and wishlist indexes
            builder.Entity<Product>()
                .HasIndex(p => p.Name);

            builder.Entity<Product>()
                .HasIndex(p => p.Brand);

            builder.Entity<Product>()
                .HasIndex(p => p.Price);

            builder.Entity<Product>()
                .HasIndex(p => p.Rating);

            builder.Entity<Wishlist>()
                .HasIndex(w => new { w.UserId, w.ProductId })
                .IsUnique();

            builder.Entity<RecentlyViewed>()
                .HasIndex(rv => new { rv.UserId, rv.ProductId });

            builder.Entity<SearchAnalytics>()
                .HasIndex(sa => sa.SearchTerm);

        }
    }
}
