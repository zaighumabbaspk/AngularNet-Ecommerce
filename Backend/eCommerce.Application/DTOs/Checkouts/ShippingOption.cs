namespace eCommerce.Application.DTOs.Checkouts
{
    public class ShippingOption
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int EstimatedDays { get; set; }
        public bool IsDefault { get; set; } = false;
    }

    public class ShippingOptionsResponse
    {
        public List<ShippingOption> Options { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
    }
}