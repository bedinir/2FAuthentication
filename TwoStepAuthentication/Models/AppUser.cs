using Microsoft.AspNetCore.Identity;

namespace TwoStepAuthentication.Models
{
    public class AppUser : IdentityUser
    {
        public string Lastname { get; set; } = "";
        public bool Is2FAEnabled { get; set; }
        public string? TwoFactorCode { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAtUtc { get; set; }
    }
}
