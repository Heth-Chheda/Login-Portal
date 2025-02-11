using Microsoft.AspNetCore.Mvc;
using LoginPortalApp.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LoginPortalApp.Controllers
{
    public class UserDetailsController : Controller
    {
        // Base address of the API you want to call
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:44396/api/userdetails"; // API URL to call

        // Constructor that initializes HttpClient
        public UserDetailsController(HttpClient client)
        {
            _client = client;
        }

        // GET: /UserDetails
        public async Task<IActionResult> Index()
        {
            try
            {
                // Call the API and get the list of user details
                var response = await _client.GetFromJsonAsync<List<UserDetailViewModel>>(_baseUrl);

                if (response != null)
                {
                    // Pass the data to the view
                    return View(response);
                }

                return View(new List<UserDetailViewModel>()); // Return an empty list if the response is null
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the API call
                ViewBag.ErrorMessage = $"Error retrieving data: {ex.Message}";
                return View(new List<UserDetailViewModel>());
            }
        }
    }
}
