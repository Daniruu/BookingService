using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<string>> RegisterAsync(RegisterDto registerDto);
        Task<ServiceResult<AuthTokenResult>> LoginAsync(LoginDto loginDto);
        Task<ServiceResult<string>> RefreshTokenAsync(string refreshToken);
    }
}
