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

        public async Task<string> CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return user.Id.ToString();
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.Message.Contains("UQ_Users.Email") == true)
                {
                    throw new Exception("Użytkownik z takim adresem Email już istnieje.");
                }
                throw;
            }
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
