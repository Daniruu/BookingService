using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly BookingServiceDbContext _context;

        public AuthRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<string> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.Id.ToString();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task RemoveExpiredRefreshTokensAsync()
        {
            var expiredTokens = _context.Users.Where(u => u.RefreshExpiryTime <= DateTime.UtcNow);
            _context.Users.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
