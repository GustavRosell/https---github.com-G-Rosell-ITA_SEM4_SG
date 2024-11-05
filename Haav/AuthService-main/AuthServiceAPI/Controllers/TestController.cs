using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        // Dette endpoint kræver godkendelse med en gyldig JWT-token
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You're authorized");
        }
    }
}
