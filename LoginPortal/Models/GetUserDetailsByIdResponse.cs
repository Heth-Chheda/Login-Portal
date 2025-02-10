using System.Text.Json.Serialization;

namespace LoginPortal.Models
{
    public class GetUserDetailsByIdResponse
    {
        public ErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        // Include a UserDetailsDto object for user-specific data
        public UserDetailsDto? UserDetails { get; set; }

        public GetUserDetailsByIdResponse(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
            ErrorMessage = GetErrorMessage(errorCode);
        }

        private string GetErrorMessage(ErrorCode errorCode)
        {
            return errorCode switch
            {
                ErrorCode.Success => "Operation successful.",
                ErrorCode.InvalidID => "The ID is not registered.",
                ErrorCode.ServerError => "An unexpected error occurred. Please try again later.",
                _ => "Unknown error occurred."
            };
        }
    }

    // DTO for user details
    public class UserDetailsDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
