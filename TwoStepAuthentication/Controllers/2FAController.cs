using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TwoStepAuthentication.Models;
using TwoStepAuthentication.Services.Interfaces;

namespace TwoStepAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _2FAController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly I2FactorAuthentication _auth;

        public _2FAController(UserManager<AppUser> userManager, I2FactorAuthentication auth)
        {
            _userManager = userManager;
            _auth = auth;
        }

        [Authorize]
        [HttpPost("enable-2fa")]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            
            var result = await _auth.EnableTwoFactorAuthentication(user);
            if (result)
            {
                return Ok("Two-factor authentication enabled. Check your email for the 2FA code.");
            }

            return BadRequest("Faild to enable two-factor authentication.");
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactorCode([FromBody] Verify2FARequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if(user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _auth.VerifyTwoFactorCode(user, request.Code);
            if (!result)
            {
                return Unauthorized("Invalid 2FA code.");
            }

            //var token = await _signInManager.CreateUserPrincipalAsync(user);
            var token = _auth.CreateToken(user).Result;

            return Ok(new
            {
                Message = "2FA verification successful.",
                Token = token // Return the generated token
            });
        }

        [Authorize]
        [HttpPost("disable-2fa")]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found.");
            }
            var result = await _auth.DisableTwoFactorAuthentication(user);
            if (result)
            {
                return Ok("Two-factor authentication disabled.");
            }

            return BadRequest("Failed to disable two-factor authentication.");
        }
    }
}
