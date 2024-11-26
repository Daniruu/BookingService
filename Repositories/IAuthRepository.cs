using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> UserExistsAsync(string email);
        Task<string> CreateUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);
        Task RemoveExpiredRefreshTokensAsync();
    }
}
