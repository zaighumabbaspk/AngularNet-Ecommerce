namespace eCommerce.Application.DTOs.Wishlist
{
    public class AddToWishlistRequest
    {
        public Guid ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class WishlistResponse
    {
        public List<WishlistItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class WishlistItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public bool IsInStock { get; set; }
        public DateTime AddedAt { get; set; }
    }
}