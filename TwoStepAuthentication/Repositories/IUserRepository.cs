using TwoStepAuthentication.Models;

namespace TwoStepAuthentication.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser?> GetUserByRefreshTokenAsync(string refreshToken);
    }
}
