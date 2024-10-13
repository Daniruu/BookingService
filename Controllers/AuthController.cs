using Microsoft.AspNetCore.Mvc;
using BookingService.Data;
using BookingService.Models;
using BookingService.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BookingServiceDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BookingServiceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    return BadRequest(new { message = "Użytkownik z takim adresem Email już istnieje" });
                }

                var user = new User
                {
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Phone = registerDto.Phone,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Role = Roles.User
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Użytkownik został pomyślnie zarejestrowany", userId = user.Id });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Błąd podczas zapisywania do bazy danych", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Nieprawidłowy email lub hasło" });
                }

                var accessToken = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshExpiryTime = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                return Ok(new { accessToken, refreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd autoryzacji", details = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var principal = GetPrincipalFromExpiredToken(refreshTokenDto.AccessToken);
                var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var userId = int.Parse(userIdClaim);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return BadRequest(new { messgae = "Użytkownik nie istnieje lub token nie prawidłowy" });
                }

                if (refreshTokenDto.RefreshToken != user.RefreshToken)
                {
                    return BadRequest(new { message = "Nieprawidłowy token" });
                }

                if (user.RefreshExpiryTime <= DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Token wygasł, proszę zalogować się ponownie" });
                }

                var newAccessToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();
                user.RefreshToken = newRefreshToken;
                user.RefreshExpiryTime = DateTime.UtcNow.AddDays(7);

                await _context.SaveChangesAsync();

                return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas aktualizacji tokena", details = ex.Message });
            }
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

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Nie prawidłowy token");
            }

            return principal;
        }
    }
}
