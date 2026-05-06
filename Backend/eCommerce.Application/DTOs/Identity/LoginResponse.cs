namespace eCommerceApp.Application.DTOs.Identity
{
    public class LoginResponse
    {
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
        public string? token { get; set; }
        public string? refreshToken { get; set; }
        public string? email { get; set; }
        public string? userId { get; set; }

        // Default constructor
        public LoginResponse() { }

        // Constructor for error messages
        public LoginResponse(string message)
        {
            this.success = false;
            this.message = message;
        }

        // Constructor for success/failure with message
        public LoginResponse(bool success, string message)
        {
            this.success = success;
            this.message = message;
        }

        // Full constructor with tokens
        public LoginResponse(bool success, string message, string token, string refreshToken)
        {
            this.success = success;
            this.message = message;
            this.token = token;
            this.refreshToken = refreshToken;
        }
    }
}
