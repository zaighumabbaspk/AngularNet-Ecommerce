namespace eCommerce.Application.DTOs
{
    public class ServiceResponse
    {
        public bool Success { get; }
        public string Message { get; }

        public ServiceResponse(bool success = false, string message = "")
        {
            Success = success;
            Message = message;
        }
    }
}
