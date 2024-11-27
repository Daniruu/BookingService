using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookingService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, IMapper mapper, IUserRepository userRepository)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ServiceResult<string>> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userRepository.UserExistsAsync(registerDto.Email))
            {
                return ServiceResult<string>.Failure("Użytkownik z takim adresem Email już istnieje.");
            }

            try
            {
                var user = _mapper.Map<User>(registerDto);

                var userId = await _authRepository.CreateUserAsync(user);

                return ServiceResult<string>.SuccessResult(userId);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<AuthTokenResult>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return ServiceResult<AuthTokenResult>.Failure("Nieprawidłowy email lub hasło.");
            }

            var accessToken = GenerateJwtToken(user);
            var refreshToken = await GenerateUniqueRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateUserAsync(user);

            return ServiceResult<AuthTokenResult>.SuccessResult(new AuthTokenResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        public async Task<ServiceResult<string>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _authRepository.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshExpiryTime <= DateTime.UtcNow)
            {
                return ServiceResult<string>.Failure("Refresh token nieprwawidłowy lub wygasł.");
            }

            var newAccessToken = GenerateJwtToken(user);

            return ServiceResult<string>.SuccessResult(newAccessToken);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<string> GenerateUniqueRefreshToken()
        {
            string refreshToken;

            do
            {
                var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    refreshToken = WebEncoders.Base64UrlEncode(randomNumber);
                }
            }
            while (await _authRepository.GetUserByRefreshTokenAsync(refreshToken) != null);

            return refreshToken;
        }
    }
}
