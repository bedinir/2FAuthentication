using Microsoft.AspNet.Identity;
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
    }
}
