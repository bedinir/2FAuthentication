using TwoStepAuthentication.Models;

namespace TwoStepAuthentication.Services
{
    public interface I2FactorAuthentication
    {
        Task<bool> EnableTwoFactorAuthentication(AppUser user);
        Task<bool> VerifyTwoFactorCode(AppUser user, string code);
        Task<bool> DisableTwoFactorAuthentication(AppUser user);
    }
}
