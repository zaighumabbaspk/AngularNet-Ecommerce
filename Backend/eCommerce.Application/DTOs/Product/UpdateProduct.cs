using eCommerce.Application.DTOs.Product;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Application.DTOs
{
    public class UpdateProduct : ProductBase
    {
        [Required]
        public Guid Id { get; set; }
    }
}
