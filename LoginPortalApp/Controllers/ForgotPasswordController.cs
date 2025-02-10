using Microsoft.AspNetCore.Mvc;
using LoginPortal.Models;
using LoginPortal.Services;
using LoginPortalApp.Models;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Web;
using System.Net.Mail;

namespace LoginPortalApp.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly IUserService _userService;
        private readonly EmailService _emailService;
        private readonly HttpClient _httpClient;
        private readonly IPasswordService _passwordService;
        private readonly TokenService _tokenService;

        public ForgotPasswordController(UserService userService, EmailService emailService, HttpClient httpClient, IPasswordService passwordService, TokenService tokenService)
        {
            _userService = userService;
            _emailService = emailService;
            _httpClient = httpClient;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        // Display the ForgotPassword page
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Handle the ForgotPassword form submission to send a reset link
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "No user found with that email address.";
                return View();
            }

            var resetToken = await _userService.GeneratePasswordResetTokenAsync(user.Email);
            var resetUrl = Url.Action("ResetPassword", "ForgotPassword", new { token = resetToken }, protocol: Request.Scheme);

            if (string.IsNullOrEmpty(resetUrl))
            {
                ViewBag.ErrorMessage = "Error generating reset link.";
                return View();
            }

            await _emailService.SendEmailAsync(user.Email, "Password Reset Request", $"To reset your password, click the link: {resetUrl}");

            ViewBag.Message = "Password reset link sent to your email.";
            return View();
        }

        // Handle sending the reset link or redirect to the ResetPassword page
        [HttpPost]
        public async Task<IActionResult> SendResetLink(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);  // Return the form with validation errors
            }

            // Sanitize email
            string email = model.Email.Trim();

            // Validate email format
            try
            {
                var mailAddress = new MailAddress(email);  // This will throw FormatException if invalid
            }
            catch (FormatException)
            {
                ModelState.AddModelError(string.Empty, "The email address is not valid.");
                return View(model);  // Return the form with error
            }

            // Check if the user exists
            var userExists = await _userService.CheckIfUserExistsAsync(email);
            if (!userExists)
            {
                ModelState.AddModelError(string.Empty, "No account found with this email.");
                return View(model);  // Return the form with error if no account is found
            }

            // Store the email in TempData to pass it to ResetPassword action
            TempData["Email"] = email;

            // Send the email to the user
            var emailBody = "Click the link to reset your password.";
            await _emailService.SendEmailAsync(email, "Password Reset Request", emailBody);

            // Redirect to ResetPassword page with email encoded in the query string
            return RedirectToAction("ResetPassword", "ForgotPassword", new { email = Uri.EscapeDataString(email) });
        }




        [HttpGet]
        public async Task<IActionResult> VerifyResetLink(string token)
        {
            // Validate the token by checking it against the Token Service
            var tokenResponse = await _tokenService.ValidateAccessTokenAsync(token);

            if (tokenResponse.ErrorCode != AccessTokenErrorCode.Success)
            {
                // Token is invalid or expired
                ViewBag.ErrorMessage = "Invalid or expired reset link.";
                return View("Error");
            }

            // If token is valid, retrieve the user's email (optional)
            var email = tokenResponse.Email;

            // Store the email in TempData or ViewData for the ResetPassword page
            TempData["Email"] = email;

            // Redirect to ResetPassword page
            return RedirectToAction("ResetPassword", "ForgotPassword");
        }




        // Display the ResetPassword page
        [HttpGet]
        public IActionResult ResetPassword()
        
        {
            // Retrieve the email from the query string
            var email = Request.Query["email"].ToString();

            if (!string.IsNullOrEmpty(email))
            {
                // Store the email in ViewData to pass it to the view
                ViewData["Email"] = email;
            }

            return View();
        }

        // Handle the ResetPassword form submission
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            _httpClient.BaseAddress = new Uri("http://loginportal:8080/");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Retrieve the email from the query string
            var email = Request.Form["email"].ToString();

            if (!string.IsNullOrEmpty(email))
            {
                // Decode the email (remove the extra URL encoding)
                email = HttpUtility.UrlDecode(email);
                Console.WriteLine($"Decoded Email: {email}");
            }

            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Email is required to reset the password.";
                return View();
            }

            // Fetch the previous passwords using the API
            var getPasswordRequest = new { email = email };
            var response = await _httpClient.PostAsJsonAsync("api/User_Details/get-passwords-by-email", getPasswordRequest);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Error fetching previous password.";
                return View();
            }

            // Deserialize the response
            var passwordResponse = await response.Content.ReadFromJsonAsync<GetPasswordByEmailResponse>();
            if (passwordResponse == null)
            {
                ViewBag.ErrorMessage = "Unexpected error occurred.";
                return View();
            }

            // Log the passwords returned from the API (for debugging)
            Console.WriteLine($"CurrentPassword: {passwordResponse?.CurrentPassword}");
            Console.WriteLine($"LastPassword: {passwordResponse?.LastPassword}");

            // Compare the plain text password with hashed passwords
            bool isCurrentPasswordMatch = _passwordService.VerifyHash(model.NewPassword, passwordResponse.CurrentPassword);
            bool isLastPasswordMatch = _passwordService.VerifyHash(model.NewPassword, passwordResponse.LastPassword);

            // Add debug messages
            Console.WriteLine($"isCurrentPasswordMatch: {isCurrentPasswordMatch}");
            Console.WriteLine($"isLastPasswordMatch: {isLastPasswordMatch}");

            if (isCurrentPasswordMatch || isLastPasswordMatch)
            {
                ModelState.AddModelError(string.Empty, "Cannot use this password. Please choose a different one.");
                return View(model);
            }

            // Proceed to reset the password
            var resetRequest = new
            {
                Email = email,
                Password = model.NewPassword
            };
            var resetResponse = await _httpClient.PostAsJsonAsync("api/Auth/password-reset", resetRequest);

            var statusCode = resetResponse.StatusCode;
            Console.WriteLine($"Reset response status: {statusCode}");

            if (resetResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Registration");
            }

            // Handle errors returned from the reset API
            var errorMessage = await resetResponse.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);
            return View(model);
        }
    }
}
