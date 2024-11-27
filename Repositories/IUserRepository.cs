using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserExistsAsync(string email);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserById(int id);
        Task UpdateUserAsync(User user);
        Task<List<Business>> GetUserFavoriteBusinesses(int userId);
        Task<bool> IsBusinessFavorite(int userId, int businessId);
        Task AddToFavorites(int userId, int businessId);
        Task RomoveFromFavorites(int userId, int businessId);
    }
}
