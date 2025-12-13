namespace eCommerceApp.Application.DTOs.Identity
{
    public class LoginResponse
    {
        public bool success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Email { get; set; }
        public string? UserId { get; set; }

        // Default constructor
        public LoginResponse() { }

        // Constructor for error messages
        public LoginResponse(string Message)
        {
            this.success = false;
            this.Message = Message;
        }

        // Constructor for success/failure with message
        public LoginResponse(bool success, string Message)
        {
            this.success = success;
            this.Message = Message;
        }

        // Full constructor with tokens
        public LoginResponse(bool success, string Message, string Token, string RefreshToken)
        {
            this.success = success;
            this.Message = Message;
            this.Token = Token;
            this.RefreshToken = RefreshToken;
        }
    }
}
