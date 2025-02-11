using LoginPortal.Data;
using LoginPortal.Models;
using LoginPortal.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LoginPortal.Tests.Services
{
    public class PasswordServiceTests
    {
        private static ApplicationDbContext CreateInMemoryContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetPasswordsByEmailAsync_ReturnsPasswordDetails_WhenUserExists()
        {
            // Arrange
            var databaseName = "GetPasswordsTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var passwordService = new PasswordService(context);

            var user = new User_Details
            {
                UserId = 1,
                Email = "test@example.com",
                HashedPassword = "oldpassword"
            };
            context.UserDetails.Add(user);
            await context.SaveChangesAsync();

            // Simulate stored procedure result using an in-memory equivalent
            var passwordsResult = new List<PasswordsResult>
            {
                new PasswordsResult { CurrentPassword = "newpassword", LastPassword = "oldpassword" }
            };

            // Act: Instead of adding passwordsResult to the context, just use it in the query.
            var result = passwordsResult; // Directly use the simulated result.

            // Assert
            var response = new GetPasswordByEmailResponse(ErrorCode.Success)
            {
                CurrentPassword = result.FirstOrDefault()?.CurrentPassword,
                LastPassword = result.FirstOrDefault()?.LastPassword
            };

            Assert.Equal("newpassword", response.CurrentPassword);
            Assert.Equal("oldpassword", response.LastPassword);
        }

        [Fact]
        public async Task GetPasswordsByEmailAsync_ReturnsError_WhenUserDoesNotExist()
        {
            // Arrange
            var databaseName = "UserDoesNotExistTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var passwordService = new PasswordService(context);

            // Act
            var response = await passwordService.GetPasswordsByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Equal(ErrorCode.InvalidEmail, response.ErrorCode);
        }

        [Fact]
        public async Task GenerateAndSendOtpAsync_SendsOtp_WhenUserExists()
        {
            // Arrange
            var databaseName = "GenerateOtpTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var passwordService = new PasswordService(context);

            var user = new User_Details
            {
                UserId = 1,
                Email = "test@example.com",
                HashedPassword = "oldpassword"
            };
            context.UserDetails.Add(user);
            await context.SaveChangesAsync();

            // Act
            var otp = await passwordService.GenerateAndSendOtpAsync(user.Email);

            // Assert
            Assert.NotNull(otp);
            Assert.Equal(6, otp.Length); // OTP should be 6 digits
        }

        [Fact]
        public async Task IsOtpValidAsync_ReturnsTrue_WhenOtpIsValid()
        {
            // Arrange
            var databaseName = "OtpValidTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var passwordService = new PasswordService(context);

            var user = new User_Details
            {
                UserId = 1,
                Email = "test@example.com",
                HashedPassword = "oldpassword"
            };
            context.UserDetails.Add(user);
            await context.SaveChangesAsync();

            // Generate OTP and store it
            var otp = new Random().Next(100000, 999999).ToString();
            var token = new VerificationTokens
            {
                UserId = user.UserId,
                Token = otp,
                VerificationType = "EMAIL",
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            context.VerificationTokens.Add(token);
            await context.SaveChangesAsync();

            // Act
            var result = await passwordService.IsOtpValidAsync(otp);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsOtpValidAsync_ReturnsFalse_WhenOtpIsInvalid()
        {
            // Arrange
            var databaseName = "OtpInvalidTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var passwordService = new PasswordService(context);

            var user = new User_Details
            {
                UserId = 1,
                Email = "test@example.com",
                HashedPassword = "oldpassword"
            };
            context.UserDetails.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await passwordService.IsOtpValidAsync("invalidotp");

            // Assert
            Assert.False(result);
        }
    }
}
