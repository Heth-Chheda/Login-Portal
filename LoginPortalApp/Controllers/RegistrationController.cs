using LoginPortal.Models;
using LoginPortalApp.Models;
using LoginPortalApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoginPortalApp.Controllers
{
    public class RegistrationController(UserService userService) : Controller
    {
        private readonly UserService _userService = userService;

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequest model)
        {
            if (ModelState.IsValid)
            {
                // Proceed with registration; the API will handle the email check automatically
                var result = await _userService.RegisterUserAsync(model);

                if (result)
                {
                    // Redirect to login page after successful registration
                    return RedirectToAction("Registration/Login");
                }
                else
                {
                    // Handle failure (e.g., show error message from API response)
                    ModelState.AddModelError(string.Empty, "Registration failed. The email might already be taken.");
                }
            }

            // Return the view with validation errors
            return View(model);
        }
        //public IActionResult Login()
        //{
        //    return View();
        //}

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result)
                {
                    // Log the role to check if it's being fetched correctly
                    var userRole = await _userService.GetUserRoleAsync(model.Email);

                    if (string.IsNullOrEmpty(userRole))
                    {
                        ModelState.AddModelError(string.Empty, "Role not found.");
                        return View(model);
                    }

                    // Log the role for debugging purposes
                    Console.WriteLine($"User role: {userRole}");

                    // Set the success message in TempData
                    TempData["SuccessMessage"] = "Login successful!";

                    // Redirect based on the user's role
                    if (userRole == "ADMIN")
                    {
                        return RedirectToAction("AdminDashboard", "Dashboard");
                    }
                    else if (userRole == "USER")
                    {
                        return RedirectToAction("UserDashboard", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid role.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }



    }
}
