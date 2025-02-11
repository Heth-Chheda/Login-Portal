namespace LoginPortal.Models
{
    public enum PasswordResetErrorCode
    {
        Success = 0,
        EmailNotFound = 1,
        PasswordResetFailed = 2,
        Unauthorized = 3,
        ServerError = 500
    }

    public class PasswordResetResponse
    {
        public PasswordResetErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public PasswordResetResponse(PasswordResetErrorCode errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public string GetResponseMessage()
        {
            // Switch case inside the response model
            return ErrorCode switch
            {
                PasswordResetErrorCode.Success => "Password reset was successful.",
                PasswordResetErrorCode.EmailNotFound => "Email is not registered.",
                PasswordResetErrorCode.PasswordResetFailed => "Password reset failed.",
                PasswordResetErrorCode.ServerError => "An error occurred while processing the request.",
                PasswordResetErrorCode.Unauthorized => "Unauthorized access Token.",
                _ => "Unknown error."
            };
        }
    }
}
