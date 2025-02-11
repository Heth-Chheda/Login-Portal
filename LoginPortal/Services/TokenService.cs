using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LoginPortal.Models;
using LoginPortal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Authentication.BearerToken;

namespace LoginPortal.Services
{
    public class TokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userServices;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(ApplicationDbContext context, IConfiguration configuration , IUserService userServices, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userServices = userServices;
            _userManager = userManager;
        }
        public async Task<AccessTokenResponse> ValidateAccessTokenAsync(string token)
        {
            var accessTokenRecord = await _context.AccessTokens
                .FirstOrDefaultAsync(t => t.AccessToken == token && t.ExpiresAt > DateTime.UtcNow);

            if (accessTokenRecord != null)
            {
                return new AccessTokenResponse
                {
                    ErrorCode = AccessTokenErrorCode.Success,
                    Email = accessTokenRecord?.User?.Email  // Retrieve the email of the user associated with the token
                };
            }

            return new AccessTokenResponse
            {
                ErrorCode = AccessTokenErrorCode.Unauthorized,
                Message = "Invalid or expired token"
            };
        }


        public async Task<GenerateAccessTokenResponse> GenerateOrRetrieveAccessTokenAsync(string email)
        {
            // Check if the user exists
            var user = await _userServices.GetUserByEmailAsync(email);
            if (user == null)
            {
                return new GenerateAccessTokenResponse(AccessTokenErrorCode.InvalidUser);  // Error if user is invalid
            }

            // Check if the user already has a valid access token
            var existingToken = await _context.AccessTokens
                .FirstOrDefaultAsync(t => t.UserId == user.UserId && t.ExpiresAt > DateTime.UtcNow);

            if (existingToken != null)
            {
                // If a valid token exists, return it from the database (success)
                return new GenerateAccessTokenResponse(AccessTokenErrorCode.Success, existingToken.AccessToken);
            }

            // If no valid token exists, generate a new token
            var newToken = await GenerateTokensAsync(user.UserId);

            if (newToken == null)
            {
                return new GenerateAccessTokenResponse(AccessTokenErrorCode.TokenGenerationFailed);  // Error if token generation failed
            }

            // Return the new access token on success
            return new GenerateAccessTokenResponse(AccessTokenErrorCode.Success, newToken.AccessToken);
        }
        // Method to generate a JWT token and a refresh token
        public async Task<AccessTokens> GenerateTokensAsync(int userId)
        {
            try
            {
                var accessToken = GenerateJwtToken(userId);  // Generate the access token
                var refreshToken = GenerateRefreshToken(userId);  // Generate the refresh token

                var issuedAt = DateTime.UtcNow;
                var accessTokenExpiry = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:AccessTokenExpiryInHours"]!)); // Use configurable expiry
                var refreshTokenExpiry = DateTime.UtcNow.AddYears(100); // Optionally set refresh token expiry

                // Log for debugging
                Console.WriteLine($"AccessTokenExpiry: {accessTokenExpiry}, IssuedAt: {issuedAt}");

                // Ensure the expiry date is within a valid range
                if (accessTokenExpiry < DateTime.UtcNow)
                {
                    throw new InvalidOperationException("The access token expiry date cannot be in the past.");
                }

                // Create and save the tokens to the database
                var token = new AccessTokens
                {
                    UserId = userId,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    IssuedAt = issuedAt,
                    ExpiresAt = accessTokenExpiry,  // Set access token expiry
                };

                Console.WriteLine($"Saving token to database for UserId: {userId}");
                _context.AccessTokens.Add(token);
                await _context.SaveChangesAsync();

                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving token to database: {ex.Message}");
                throw;
            }
        }

        private string GenerateJwtToken(int userId)
        {
            var secretKey = _configuration["Jwt:SecretKey"];  // Retrieve the secret key from configuration
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT Secret Key is not configured properly.");
            }

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),  // Subject (user ID)
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // JWT ID (unique)
        new Claim("user_id", userId.ToString())  // Custom claim for user ID
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:AccessTokenExpiryInHours"]!)), // 100 years
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);  // Generate the JWT string
        }



        // Helper method to generate a refresh token (no expiry time in claims)
        private string GenerateRefreshToken(int userId)
        {
            var refreshToken = Guid.NewGuid().ToString();  // Generate a GUID for the refresh token
            return refreshToken;
        }
    }
}
