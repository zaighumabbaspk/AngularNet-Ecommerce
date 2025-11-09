using eCommerce.Application.DTOs;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceApp.Application.Validations
{
    public interface IValidationService
    {
        Task<ServiceResponse> ValidateAsync<T>(T model, IValidator<T> validator);
    }
}
