using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Domain.Exceptions
{


    public class ItemNotFound(string message) : Exception(message)
    {
    }
}

