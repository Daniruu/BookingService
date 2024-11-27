using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IAuthRepository
    {
        Task<string> CreateUserAsync(User user);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);
        Task RemoveExpiredRefreshTokensAsync();
    }
}
