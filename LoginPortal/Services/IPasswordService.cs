using LoginPortal.Models;

namespace LoginPortal.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        Task<string> GenerateAndSendOtpAsync(string email);
        Task<bool> IsOtpValidAsync(string otp);
        Task<GetPasswordByEmailResponse> GetPasswordsByEmailAsync(string email);
        bool VerifyHash(string plainTextPassword, string hashedPassword);
    }
}
