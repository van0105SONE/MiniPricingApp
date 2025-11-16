using Microsoft.AspNetCore.Mvc;

namespace MiniPricingApp.Modules.Health
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : Controller
    {
        [HttpGet]
       public IActionResult HealthCheck()
        {
            return Ok("I'm good❤️👌");
        }
    }
}
