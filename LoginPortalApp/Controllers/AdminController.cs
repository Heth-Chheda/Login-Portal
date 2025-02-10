using Microsoft.AspNetCore.Mvc;
using LoginPortalApp.Services;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using LoginPortal.Controllers;

namespace LoginPortalApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;

        public AdminController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public IActionResult ManageUsers()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByID(int id)
        {
            _logger.LogInformation($"Fetching user details for ID: {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid user ID: {id}");
                return BadRequest("Invalid user ID.");
            }

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning($"User not found for ID: {id}");
                return NotFound("User not found.");
            }

            _logger.LogInformation($"User details retrieved: {JsonConvert.SerializeObject(user)}");
            return Json(user);
        }

    }
}
