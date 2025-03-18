using TwoStepAuthentication.Models;

namespace TwoStepAuthentication.Services.Interfaces
{
    public interface I2FactorAuthentication
    {
        Task<bool> EnableTwoFactorAuthentication(AppUser user);
        Task<bool> VerifyTwoFactorCode(AppUser user, string code);
        Task<bool> DisableTwoFactorAuthentication(AppUser user);
        Task<string> GenerateAndSendTwoFactorTokenAsync(AppUser user);
        Task<(string jwtToken, DateTime expiresAtUtc)> CreateToken(AppUser user);
        string GenerateRefreshToken();
        Task RefreshTokenAsync(string? refreshToken);
        void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
    }
}
