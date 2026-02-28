using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/test")]  // 
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API is running");
        }
    }
}
