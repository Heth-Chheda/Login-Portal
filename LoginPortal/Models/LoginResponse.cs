namespace LoginPortal.Models
{
    public class LoginResponse
    {
        public ErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public LoginResponse(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
            ErrorMessage = GetErrorMessage(errorCode);  // Get the message from the error code
        }

        // Method to get the error message based on the error code
        private string GetErrorMessage(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.Success:
                    return "Login successful!";
                case ErrorCode.InvalidEmail:
                    return "The email address is not registered.";
                case ErrorCode.InvalidPassword:
                    return "The password you entered is incorrect.";
                case ErrorCode.ServerError:
                    return "An internal server error occurred. Please try again later.";
                default:
                    return "An unknown error occurred.";
            }
        }
    }
}
