namespace LoginPortal.Models
{
    // Enum specific to RegistrationResponse
    public enum ErrorCode
    {
        Success = 0,
        InvalidEmail = 1,
        InvalidPassword = 2,
        InvalidPhoneNumber = 3,
        InvalidAddress = 4,
        InvalidID = 5,
        ServerError = 500
    }

    public class RegistrationResponse
    {
        public ErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        // Constructor for error response without access token
        public RegistrationResponse(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
            ErrorMessage = GetErrorMessage(errorCode); // Calls the switch case to set the message
        }

        // Method that returns error messages based on the ErrorCode
        private string GetErrorMessage(ErrorCode errorCode)
        {
            return errorCode switch
            {
                ErrorCode.Success => "Registration successful.",
                ErrorCode.InvalidEmail => "Email is already taken.",
                ErrorCode.InvalidPassword => "Password is invalid.",
                ErrorCode.InvalidPhoneNumber => "Phone number is invalid.",
                ErrorCode.InvalidAddress => "Address is invalid.",
                ErrorCode.ServerError => "An unexpected error occurred. Please try again later.",
                _ => "Unknown error occurred."
            };
        }
    }
}
