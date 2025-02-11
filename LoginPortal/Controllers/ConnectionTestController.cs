using LoginPortal.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionTestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConnectionTestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            var canConnect = await _context.CanConnectAsync();
            if (canConnect)
            {
                return Ok("Connection to the database is successful.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to connect to the database.");
            }
        }
    }
}
