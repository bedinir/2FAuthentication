using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TwoStepAuthentication.Models;
using TwoStepAuthentication.Services.Interfaces;

namespace TwoStepAuthentication.Services
{
    public class _2FactorAuthentication : I2FactorAuthentication
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        // we have to create a symmetric security kee so that we can sign the token 
        private readonly SymmetricSecurityKey _key;
        public _2FactorAuthentication(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"].ToCharArray()));
        }

        public async Task<(string jwtToken, DateTime expiresAtUtc)> CreateToken(AppUser user)
        {
            // A token contains claims for a user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName, user.Lastname),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDiscription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDiscription);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName,
                token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = expiration,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
        }

        public async Task<bool> DisableTwoFactorAuthentication(AppUser user)
        {
            user.Is2FAEnabled = false;
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

        public async Task<string> GenerateAndSendTwoFactorTokenAsync(AppUser user)
        {
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
            if(!string.IsNullOrEmpty(token))
            {
                await _emailSender.SendEmailAsync(user.Email, "Your 2FA Code", $"Your 2FA code is {token}");
            }
            return token;
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
