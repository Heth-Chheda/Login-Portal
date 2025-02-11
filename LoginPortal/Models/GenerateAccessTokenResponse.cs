namespace LoginPortal.Models
{
    public enum AccessTokenErrorCode
    {
        Success = 0,
        Unauthorized = 1,
        InvalidUser = 2,
        ExistingTokenReturned = 3,
        TokenGenerationFailed = 4
    }
    public class GenerateAccessTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;  // Only the token is needed on success
        public AccessTokenErrorCode ErrorCode { get; set; }    // Error code for failure cases
        public string Message { get; set; } = string.Empty;     // Error message based on the error code

        public GenerateAccessTokenResponse() { }  // Default constructor

        // Constructor for success (with access token)
        public GenerateAccessTokenResponse(AccessTokenErrorCode errorCode, string accessToken)
        {
            AccessToken = accessToken;
            ErrorCode = errorCode;
            Message = GetMessageFromErrorCode(errorCode);
        }

        // Constructor for failure (with error code and message)
        public GenerateAccessTokenResponse(AccessTokenErrorCode errorCode)
        {
            ErrorCode = errorCode;
            Message = GetMessageFromErrorCode(errorCode);
        }

        private string GetMessageFromErrorCode(AccessTokenErrorCode errorCode)
        {
            return errorCode switch
            {
                AccessTokenErrorCode.Success => "New access token generated successfully.",
                AccessTokenErrorCode.Unauthorized => "User is not authenticated.",
                AccessTokenErrorCode.InvalidUser => "Invalid user.",
                AccessTokenErrorCode.ExistingTokenReturned => "Existing access token returned.",
                AccessTokenErrorCode.TokenGenerationFailed => "Failed to generate access token.",
                _ => "An unknown error occurred."
            };
        }
    }
}
