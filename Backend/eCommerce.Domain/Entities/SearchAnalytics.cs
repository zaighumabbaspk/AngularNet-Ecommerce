namespace eCommerce.Domain.Entities
{
    public class SearchAnalytics
    {
        public Guid Id { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int SearchCount { get; set; }
        public int ResultCount { get; set; }
        public DateTime LastSearched { get; set; }
        public string? UserId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }
}