namespace LoginPortal.Models
{
    public class AccessTokenResponse
    {
        public AccessTokenErrorCode ErrorCode { get; set; }
        public string? AccessToken { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
    }
}
