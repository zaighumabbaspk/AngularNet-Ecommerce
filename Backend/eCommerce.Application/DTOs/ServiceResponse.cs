namespace eCommerce.Application.DTOs
{
    public class ServiceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ServiceResponse(bool success = false, string message = "")
        {
            Success = success;
            Message = message;
        }
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public T? Data { get; set; }

        public ServiceResponse(bool success = false, string message = "", T? data = default)
            : base(success, message)
        {
            Data = data;
        }
    }
}
