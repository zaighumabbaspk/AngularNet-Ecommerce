using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Application.DTOs.Product
{
    public class GetProduct: ProductBase
    {
        [Required(ErrorMessage = "")]
        public Guid Id { get; set; }

        public Guid CategoryId { get; set; }
    }
}
