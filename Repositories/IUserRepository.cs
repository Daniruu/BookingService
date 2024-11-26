using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserExistsAsync(string email);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserById(int id);
        Task UpdateUserAsync(User user);

    }
}
