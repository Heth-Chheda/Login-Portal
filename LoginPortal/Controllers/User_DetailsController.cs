using Azure;
using LoginPortal.Data;
using LoginPortal.Models;
using LoginPortal.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Threading.Tasks;

namespace LoginPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User_DetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();  // NLog logger

        public User_DetailsController(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDetailsByIdResponse>> GetUserDetails(int id)
        {
            _logger.Info($"Fetching user details for User ID: {id}");  // Log request

            var userDetails = await _context.UserDetails.FindAsync(id);

            if (userDetails == null)
            {
                _logger.Warn($"User not found for ID: {id}");  // Log a warning if user not found
                return NotFound(new GetUserDetailsByIdResponse(ErrorCode.InvalidID));
            }

            _logger.Info($"Successfully fetched user details for User ID: {id}");  // Log success

            // Create a success response that includes user details
            var response = new GetUserDetailsByIdResponse(ErrorCode.Success)
            {
                UserDetails = new UserDetailsDto
                {
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    PhoneNumber = userDetails.PhoneNumber,
                    Address = userDetails.Address,
                    DateOfBirth = userDetails.DateOfBirth
                }
            };

            return Ok(response);
        }

        // PUT: api/UserDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDetails(int id, User_Details userDetails)
        {
            _logger.Info($"Updating user details for User ID: {id}");  // Log request

            if (id != userDetails.UserId)
            {
                _logger.Error($"User ID mismatch: Requested ID = {id}, Provided ID = {userDetails.UserId}");  // Log error for mismatch
                return BadRequest();
            }

            try
            {
                _context.Entry(userDetails).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.Info($"Successfully updated user details for User ID: {id}");  // Log success
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error updating user details for User ID: {id}");  // Log exception on error
                return StatusCode(500, "Internal server error");
            }

            return NoContent();
        }

        // DELETE: api/UserDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDetails(int id)
        {
            _logger.Info($"Deleting user details for User ID: {id}");  // Log request

            var userDetails = await _context.UserDetails.FindAsync(id);
            if (userDetails == null)
            {
                _logger.Warn($"User not found for ID: {id}");  // Log a warning if user not found
                return NotFound();
            }

            _context.UserDetails.Remove(userDetails);
            await _context.SaveChangesAsync();

            _logger.Info($"Successfully deleted user details for User ID: {id}");  // Log success
            return NoContent();
        }

        [HttpPost("get-passwords-by-email")]
        public async Task<ActionResult<GetPasswordByEmailResponse>> GetPasswordsByEmail(GetPasswordByEmailRequest request)
        {
            _logger.Info($"Fetching passwords for email: {request.Email}");  // Log request

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                _logger.Warn($"Invalid email provided: {request.Email}");  // Log invalid email warning
                return BadRequest(new GetPasswordByEmailResponse(ErrorCode.InvalidEmail));
            }

            var response = await _passwordService.GetPasswordsByEmailAsync(request.Email);

            if (response.ErrorCode == ErrorCode.InvalidEmail)
            {
                _logger.Warn($"Email not found: {request.Email}");  // Log email not found warning
                return NotFound(response);
            }

            _logger.Info($"Successfully fetched passwords for email: {request.Email}");  // Log success
            return Ok(response);
        }
    }
}
