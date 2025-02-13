using LoginPortal.Models;
using LoginPortalApp.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using LoginPortal.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LoginPortal.Services;
using Microsoft.AspNetCore.Identity;

namespace LoginPortalApp.Services
{
    public class UserService
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext? _dbContext;
        private readonly LoginPortal.Services.IUserService _userService;
        private readonly TokenService _tokenService;

        // Constructor accepts HttpClient, which is already registered in DI container
        public UserService(HttpClient client, ApplicationDbContext context, LoginPortal.Services.IUserService userService, TokenService tokenService)
        {
            _client = client;
            _dbContext = context;
            _userService = userService;
            _tokenService = tokenService;
            
        }

        // Register user by calling the API
        public async Task<bool> RegisterUserAsync(RegistrationRequest model)
        {
            // Serialize the model to JSON
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Use the base address combined with the specific endpoint
            var response = await _client.PostAsync("/api/Auth/register", content);

            // Check if the response is successful and return appropriate result
            if (response.IsSuccessStatusCode)
            {
                return true;  // Registration successful
            }

            // If the response is not successful, handle the failure (e.g., email already taken)
            var responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody.Contains("Email is already taken"))
            {
                return false; // Email is already taken
            }

            // Handle any other failure case (generic error or unknown issue)
            return false;
        }

        public async Task<bool> LoginUserAsync(LoginViewModel model)
        {
            // Create the login request payload
            var loginRequest = new
            {
                Email = model.Email,
                Password = model.Password // Ensure password is part of the model
            };

            // Serialize the login request
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Call the external API for login validation
            var response = await _client.PostAsync("/api/Auth/Login", content);

            if (response.IsSuccessStatusCode)
            {
                // If login is successful, parse the response
                var responseBody = await response.Content.ReadAsStringAsync();

                // Assuming the API returns a success flag or similar
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseBody);

                // Check if login was successful
                if (loginResponse?.ErrorCode == ErrorCode.Success)
                {
                    // Optionally store session data or handle further actions
                    return true;
                }
            }

            // Login failed
            return false;
        }


        public async Task<bool> VerifyAccessTokenAsync(string accessToken)
        {
            // Prepare the token verification request
            var verifyRequest = new { AccessToken = accessToken };
            var content = new StringContent(JsonConvert.SerializeObject(verifyRequest), Encoding.UTF8, "application/json");

            // Call the Generate-Access-Token API to verify the access token
            var verifyResponse = await _client.PostAsync("/api/Auth/Generate-Access-Token", content);

            if (verifyResponse.IsSuccessStatusCode)
            {
                var verifyResponseBody = await verifyResponse.Content.ReadAsStringAsync();
                var verifyResult = JsonConvert.DeserializeObject<TokenVerificationResponse>(verifyResponseBody);

                // Return true if the token is valid
                return verifyResult?.IsValid ?? false;
            }

            return false;  // Token verification failed
        }


        public async Task<string?> GetUserRoleAsync(string email)
        {
            // Fetch user by email
            var user = await _dbContext.UserDetails
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null; // Return null if user is not found
            }

            return user.Role; // Make sure the Role field exists and is being correctly set
        }

        public class TokenVerificationResponse
        {
            public bool IsValid { get; set; }
            public string? ErrorMessage { get; set; }
        }

        public async Task<UserDetailViewByIdModel?> GetUserByIdAsync(int id)
        {
            var response = await _client.GetAsync($"/api/User_Details/{id}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<GetUserDetailsByIdResponse>(responseBody);

                if (apiResponse?.ErrorCode == ErrorCode.Success)
                {
                    return new UserDetailViewByIdModel
                    {
                        UserId = id,
                        Email = apiResponse.UserDetails?.Email ?? string.Empty,
                        FirstName = apiResponse.UserDetails?.FirstName ?? string.Empty,
                        LastName = apiResponse.UserDetails?.LastName ?? string.Empty,
                        PhoneNumber = apiResponse.UserDetails?.PhoneNumber ?? string.Empty,
                        Address = apiResponse.UserDetails?.Address ?? string.Empty,
                        DateOfBirth = apiResponse.UserDetails?.DateOfBirth ?? DateTime.MinValue
                    };
                }
            }

            return null;
        }


    }
}
