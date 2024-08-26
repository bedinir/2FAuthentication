using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TwoStepAuthentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public string Get()
        {
            return "Hello " + User.Identity!.Name;
        }
    }
}
