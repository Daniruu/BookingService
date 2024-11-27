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

        public async Task<List<Business>> GetUserFavoriteBusinesses(int userId)
        {
            return await _context.UserFavorites
                    .Where(uf => uf.UserId == userId)
                    .Include(uf => uf.Business.Images)
                    .Select(uf => uf.Business)
                    .ToListAsync();
        }

        public async Task<bool> IsBusinessFavorite(int userId, int businessId)
        {
            return await _context.UserFavorites.AnyAsync(uf => uf.UserId == userId && uf.BusinessId == businessId);
        }
        public async Task AddToFavorites(int userId, int businessId)
        {
            _context.UserFavorites.Add(new UserFavorite
            {
                UserId = userId,
                BusinessId = businessId
            });
            await _context.SaveChangesAsync();
        }
        public async Task RomoveFromFavorites(int userId, int businessId)
        {
            var favorite = await _context.UserFavorites.FirstOrDefaultAsync(uf => uf.UserId == userId && uf.BusinessId == businessId);

            if (favorite != null)
            {
                _context.UserFavorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }
    }
}
