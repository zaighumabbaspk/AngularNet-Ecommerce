public class GetCart
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public List<GetCartItem> CartItems { get; set; } = new();
    public decimal Total { get; set; }
    public int TotalItems { get; set; }
}