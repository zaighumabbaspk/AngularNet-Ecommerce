using System;

namespace eCommerce.Application.DTOs.Cart
{
    public class UpdateCartItemRequest
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
