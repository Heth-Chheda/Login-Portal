namespace LoginPortal.Models
{
    public class GetPasswordByEmailResponse(ErrorCode errorCode)
    {
        public ErrorCode ErrorCode { get; set; } = errorCode;
        public string? CurrentPassword { get; set; }
        public string? LastPassword { get; set; }
    }


}
