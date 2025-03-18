using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwoStepAuthentication.Models;
using TwoStepAuthentication.Services;
using TwoStepAuthentication.Services.Interfaces;

namespace TwoStepAuthentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly I2FactorAuthentication _2fauth;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, I2FactorAuthentication auth, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _2fauth = auth;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<ResponseData<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if(!ModelState.IsValid)
            {
                return new ResponseData<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid login request."
                }; 
            }

            var response= await _authService.Login(request);
            if(!response.Success)
            {
                return new ResponseData<LoginResponse>
                {
                    Success = false,
                    Message = response.Message
                };
            }

            if (response.Data.Is2FAEnabled)
            {
                return new ResponseData<LoginResponse>
                {
                    Success = true,
                    Message = "Two-factor authentication required.",
                    Data = new LoginResponse
                    {
                        UserId = response.Data.UserId,
                        UserName = response.Data.UserName,
                        Is2FAEnabled = response.Data.Is2FAEnabled
                    }
                };
            }

            return new ResponseData<LoginResponse>
            {
                Success = response.Success,
                Message = response.Message,
                Data = new LoginResponse
                {
                    UserId = response.Data.UserId,
                    UserName = response.Data.UserName,
                    Token = response.Data.Token,
                    Is2FAEnabled = response.Data.Is2FAEnabled
                }
            };
        }

        [HttpPost("register")]
        public async Task<ResponseData<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseData<RegisterResponse>
                {
                    Success = false,
                    Message = "Invalid registration request."
                };
            }
            var response = await _authService.Register(request);
            if (!response.Success)
            {
                return new ResponseData<RegisterResponse>
                {
                    Success = false,
                    Message = response.Message
                };
            }
            return new ResponseData<RegisterResponse>
            {
                Success = response.Success,
                Message = response.Message,
                Data = new RegisterResponse
                {
                    UserId = response.Data.UserId,
                    UserName = response.Data.UserName
                }
            };
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.LogoutAsync();
            if (result)
            {
                return Ok("Logout successful.");
            }
            return BadRequest("Logout failed.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
            if (result)
            {
                return Ok("Password reset successful.");
            }
            return BadRequest("Error resetting password.");
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _authService.ChangePasswordAsync(request.CurrentPassword, request.NewPassword, user);
            if (result)
            {
                return Ok("Password change successful.");
            }
            return BadRequest("Error changing password.");
        }
    }
}
