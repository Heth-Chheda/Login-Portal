using LoginPortal.Data;
using LoginPortal.Models;
using LoginPortal.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;

namespace LoginPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IUserService userServices, IPasswordService passwordServices, TokenService tokenServices) : ControllerBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IUserService _userServices = userServices;
        private readonly IPasswordService _passwordServices = passwordServices;
        private readonly TokenService _tokenService = tokenServices;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
        {
            try
            {
                if (registrationRequest == null)
                    return BadRequest("Invalid registration request.");

                _logger.Info("Register endpoint hit.");
                if (await _userServices.IsEmailTakenAsync(registrationRequest.Email))
                {
                    _logger.Warn($"Registration failed. Email {registrationRequest.Email} is already taken.");
                    return BadRequest(new RegistrationResponse(ErrorCode.InvalidEmail));
                }

                var userDetails = new User_Details
                {
                    Email = registrationRequest.Email,
                    HashedPassword = _passwordServices.HashPassword(registrationRequest.Password),
                    FirstName = registrationRequest.FirstName,
                    LastName = registrationRequest.LastName,
                    PhoneNumber = registrationRequest.PhoneNumber,
                    Address = registrationRequest.Address,
                    DateOfBirth = registrationRequest.DateOfBirth,
                };

                await _userServices.AddUserAsync(userDetails);
                _logger.Info($"User with ID {userDetails.UserId} registered successfully.");
                return Ok(new RegistrationResponse(ErrorCode.Success));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An exception occurred during registration.");
                return StatusCode(500, new RegistrationResponse(ErrorCode.ServerError));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null)
                    return BadRequest("Invalid login request.");

                _logger.Info("Login endpoint hit.");
                var user = await _userServices.GetUserByEmailAsync(loginRequest.Email);

                if (user == null)
                {
                    _logger.Warn($"Login failed. Email {loginRequest.Email} not found.");
                    return Unauthorized(new LoginResponse(ErrorCode.InvalidEmail));
                }

                if (!_passwordServices.VerifyPassword(loginRequest.Password, user.HashedPassword))
                {
                    _logger.Warn($"Login failed. Incorrect password for email {loginRequest.Email}.");
                    return Unauthorized(new LoginResponse(ErrorCode.InvalidPassword));
                }

                _logger.Info($"Login successful for user {loginRequest.Email}.");
                return Ok(new LoginResponse(ErrorCode.Success));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An exception occurred during login.");
                return StatusCode(500, new LoginResponse(ErrorCode.ServerError));
            }
        }

        [HttpPost("generate-access-token")]
        public async Task<IActionResult> GenerateAccessToken([FromBody] GenerateAccessTokenRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request.");

                _logger.Info($"Access token generation requested for email {request.Email}.");
                var response = await _tokenService.GenerateOrRetrieveAccessTokenAsync(request.Email);

                if (response.ErrorCode == AccessTokenErrorCode.Success)
                {
                    _logger.Info($"Access token generated successfully for email {request.Email}.");
                    return Ok(new { AccessToken = response.AccessToken });
                }
                else
                {
                    _logger.Warn($"Access token generation failed for email {request.Email}. Error: {response.ErrorCode}");
                    return Unauthorized(new { response.ErrorCode, response.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An exception occurred during access token generation.");
                return StatusCode(500, "Server error occurred.");
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly UserService _userServices;
        private readonly EmailService _emailService;

        public UserController(UserService userServices, EmailService emailService)
        {
            _userServices = userServices;
            _emailService = emailService;
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request.");

                _logger.Info($"Password reset link requested for email {request.Email}.");
                var user = await _userServices.GetUserByEmailAsync(request.Email);

                if (user == null)
                {
                    _logger.Warn($"Password reset link failed. Email {request.Email} not found.");
                    return NotFound(new { errorCode = "EmailNotFound", message = "No user found with that email address." });
                }

                var resetToken = await _userServices.GeneratePasswordResetTokenAsync(user.Email);
                var resetUrl = Url.Action("ResetPassword", "ForgotPassword", new { token = resetToken }, Request.Scheme);
                await _emailService.SendEmailAsync(user.Email, "Password Reset Request", $"Click the link to reset your password: {resetUrl}");

                _logger.Info($"Password reset link sent to email {user.Email}.");
                return Ok(new { message = "A password reset link has been sent to your email address." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error occurred while requesting password reset for email {request.Email}.");
                return StatusCode(500, new { errorCode = "ServerError", message = "An error occurred while processing your request." });
            }
        }
    }
}
