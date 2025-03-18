using Microsoft.EntityFrameworkCore;
using TwoStepAuthentication.Models;

namespace TwoStepAuthentication.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<AppUser?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            return user;
        }
    }
}
