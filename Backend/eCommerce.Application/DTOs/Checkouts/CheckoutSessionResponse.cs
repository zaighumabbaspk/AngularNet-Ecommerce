using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Application.DTOs.Checkouts
{
    public class CheckoutSessionResponse
    {
        public string SessionId { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    
}
}
