using Microsoft.AspNetCore.Mvc;

namespace LoginPortalApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult AdminDashboard()
        {
            return View();
        }
        public IActionResult UserDashboard()
        {
            return View();
        }
    }
}
