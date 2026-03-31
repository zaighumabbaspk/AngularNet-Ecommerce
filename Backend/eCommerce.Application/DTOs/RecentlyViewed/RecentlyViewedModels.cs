namespace eCommerce.Application.DTOs.RecentlyViewed
{
    public class AddRecentlyViewedRequest
    {
        public Guid ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class RecentlyViewedResponse
    {
        public List<RecentlyViewedItem> Items { get; set; } = new();
    }

    public class RecentlyViewedItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; }
    }
}