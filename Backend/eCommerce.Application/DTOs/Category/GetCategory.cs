using eCommerce.Application.DTOs.Category;
using eCommerce.Application.DTOs.Product;

public class GetCategory : CategoryBase

{
    public Guid Id { get; set; }

    public ICollection<GetProduct> Products { get; set; }

   
}
