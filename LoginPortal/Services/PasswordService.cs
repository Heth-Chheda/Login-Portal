using LoginPortal.Models;
using LoginPortal.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LoginPortal.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly ApplicationDbContext _context;

        // Constructor
        public PasswordService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hash a password
        public virtual string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        // Verify if the provided password matches the hashed password
        public virtual bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        // Generate and send OTP
        public async Task<string> GenerateAndSendOtpAsync(string email)
        {
            var user = await _context.UserDetails.FirstOrDefaultAsync(u => u.Email == email) ?? throw new Exception("User not found");

            // Generate a 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Create a new verification token
            var token = new VerificationTokens
            {
                UserId = user.UserId,
                Token = otp,
                VerificationType = "EMAIL",
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5) // OTP valid for 5 minutes
            };

            _context.VerificationTokens.Add(token);
            await _context.SaveChangesAsync();

            // Send the OTP (implementation depends on your email service)
            await SendOtpAsync(user.Email, otp);

            return otp; // Return the OTP for testing or debugging (remove in production)
        }

        private async Task SendOtpAsync(string email, string otp)
        {
            // Replace this with your email sending service logic
            Console.WriteLine($"Sending OTP {otp} to email {email}");
            await Task.CompletedTask; // Simulate async email sending
        }

        // Check if OTP is valid
        public async Task<bool> IsOtpValidAsync(string otp)
        {
            var token = await _context.VerificationTokens
                .Where(t => t.Token == otp && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (token == null) return false;

            token.IsUsed = true; // Mark the OTP as used
            await _context.SaveChangesAsync();

            return true;
        }

        // Get passwords by email using stored procedure
        public async Task<GetPasswordByEmailResponse> GetPasswordsByEmailAsync(string email)
        {
            var user = await _context.UserDetails
                                        .Where(u => u.Email == email)
                                        .FirstOrDefaultAsync();

            if (user == null)
            {
                return new GetPasswordByEmailResponse(ErrorCode.InvalidEmail);
            }

            // Execute the stored procedure to fetch both current and last password
            var passwords = await _context
                .Set<PasswordsResult>() // Use the PasswordsResult DTO to capture the procedure result
                .FromSqlRaw("EXEC GetCurrentAndLastPassword @UserID = {0}", user.UserId)
                .ToListAsync();

            // If no passwords are found, return an error
            if (passwords == null || !passwords.Any())
            {
                return new GetPasswordByEmailResponse(ErrorCode.InvalidEmail);
            }

            var currentPassword = passwords.FirstOrDefault()?.CurrentPassword;
            var lastPassword = passwords.FirstOrDefault()?.LastPassword;

            // Return the response with both passwords
            var response = new GetPasswordByEmailResponse(ErrorCode.Success)
            {
                CurrentPassword = currentPassword,
                LastPassword = lastPassword
            };

            return response;
        }

        // Verify the password hash
        public bool VerifyHash(string plainTextPassword, string hashedPassword)
        {
            // Hash the plain password entered by the user
            var bytes = Encoding.UTF8.GetBytes(plainTextPassword);
            var hash = SHA256.HashData(bytes);

            // Convert the hashed bytes to a Base64 string
            string hashedInput = Convert.ToBase64String(hash);

            // Compare the generated hash with the stored hash
            return hashedInput == hashedPassword;
        }
    }
}
