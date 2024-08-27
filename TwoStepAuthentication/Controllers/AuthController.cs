using Microsoft.AspNetCore.Mvc;
using TwoStepAuthentication.Models;
using TwoStepAuthentication.Services;
using TwoStepAuthentication.Services.Interfaces;

namespace TwoStepAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly I2FactorAuthentication _auth;

        public AuthController(IAuthService authService, I2FactorAuthentication auth)
        {
            _authService = authService;
            _auth = auth;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid login request.");
            }

            var response= await _authService.Login(request);
            if(!response.Success)
            {
                return Unauthorized(response.Message);
            }

            if (response.Data.Is2FAEnabled)
            {
                return Ok(new
                {
                    response.Message,
                    response.Data.UserId,
                    response.Data.UserName,
                    response.Data.Is2FAEnabled
                });
            }

            return Ok(new
            {
                response.Message,
                Token = response.Data.Token,
                response.Data.UserId,
                response.Data.UserName
            });
        }
    }
}
