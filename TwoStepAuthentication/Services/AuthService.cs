using Microsoft.AspNetCore.Identity;
using TwoStepAuthentication.Models;
using TwoStepAuthentication.Services.Interfaces;

namespace TwoStepAuthentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly I2FactorAuthentication _factorAuthentication;
        public AuthService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,I2FactorAuthentication factorAuthentication)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _factorAuthentication = factorAuthentication;
        }

        public async Task<ResponseData<LoginResponse>> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);
            if (user == null)
            {
                return new ResponseData<LoginResponse>
                {
                    Success = false,
                    Message = "User not found"
                };
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password,false, false);
            if (!result.Succeeded)
            {
                return new ResponseData<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }
            if (user.Is2FAEnabled)
            {
                var code = _factorAuthentication.GenerateAndSendTwoFactorTokenAsync(user);
                return new ResponseData<LoginResponse>
                {
                    Success = true,
                    Message = "Two-factor authentication required.",
                    Data = new LoginResponse
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Is2FAEnabled = true
                    }
                };
            }

            var token = await _factorAuthentication.CreateToken(user);
            return new ResponseData<LoginResponse>
            {
                Success = true,
                Message = "Login successful.",
                Data = new LoginResponse
                {
                    Token = token,
                    UserId = user.Id,
                    UserName = user.UserName,
                    Is2FAEnabled = false
                }
            };
        }

        public async Task<bool> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            //var user = await _userManager.FindByEmailAsync(email);
            //if(user == null)
            //{
            //    return false;
            //}
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return false;
            }
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, AppUser user)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }
    }
}
