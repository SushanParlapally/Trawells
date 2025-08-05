using Microsoft.AspNetCore.Mvc;

namespace TravelDesk.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "healthy",
                message = "Travel Desk API is running",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                endpoints = new[]
                {
                    "/api/Login",
                    "/api/Users",
                    "/api/Department",
                    "/api/TravelRequest",
                    "/api/Project",
                    "/api/Role"
                }
            });
        }
    }
} 