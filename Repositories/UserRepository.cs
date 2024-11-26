using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;
namespace BookingService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BookingServiceDbContext _context;

        public UserRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
