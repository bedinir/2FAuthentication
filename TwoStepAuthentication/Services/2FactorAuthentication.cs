using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using TwoStepAuthentication.Models;

namespace TwoStepAuthentication.Services
{
    public class _2FactorAuthentication : I2FactorAuthentication
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public _2FactorAuthentication(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task<bool> DisableTwoFactorAuthentication(AppUser user)
        {
            user.TwoFactorEnabled = false;
            user.TwoFactorCode = null;
            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<bool> EnableTwoFactorAuthentication(AppUser user)
        {
            // Generate 2FA token
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

            // Update user to enable 2FA
            user.Is2FAEnabled = true;
            user.TwoFactorCode = token;
            await _userManager.UpdateAsync(user);

            // Send the token to the user (via email, SMS, etc)
            await _emailSender.SendEmailAsync(user.Email, "Your 2FA Code", $"Your 2FA code is {token}");

            return true;
        }

        public async Task<bool> VerifyTwoFactorCode(AppUser user, string code)
        {
            if (user == null || !user.Is2FAEnabled)
                return false;
            // Verify the token
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, code);
            if (isValid)
            {
                await _signInManager.CheckPasswordSignInAsync(user, user.PasswordHash,true);
                return true;
            }
            return false;
        }
    }
}
