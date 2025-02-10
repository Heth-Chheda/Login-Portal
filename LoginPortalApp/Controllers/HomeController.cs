using LoginPortalApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LoginPortalApp.Services;

namespace LoginPortalApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserService _userService;
        public HomeController(ILogger<HomeController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        // GET: Home/Index - The landing page where users can either login or register.
        public IActionResult Index()
        {
            return View();  // Return the Index.cshtml view.
        }

        public System.Security.Claims.ClaimsPrincipal GetUser()
        {
            return User;
        }

        // Default dashboard route that redirects based on user role
        public async Task<IActionResult> Dashboard()
        {
            var email = User.Identity.Name; // Assuming email is stored in the Identity Name
            var role = await _userService.GetUserRoleAsync(email);

            if (role == "ADMIN")
            {
                return RedirectToAction("AdminDashboard", "Dashboard");
            }

            return RedirectToAction("UserDashboard", "Dashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
