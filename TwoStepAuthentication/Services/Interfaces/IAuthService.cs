using TwoStepAuthentication.Models;

namespace TwoStepAuthentication.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseData<LoginResponse>> Login(LoginRequest loginRequest);
        Task<ResponseData<RegisterResponse>> Register(RegisterRequest registerRequest);
        Task<bool> LogoutAsync();
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, AppUser user);
        Task<bool> AssignRole(string email, string roleName);
    }
}
