using Microsoft.AspNetCore.Mvc;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using BookingService.Services;
using AutoMapper.QueryableExtensions;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/users")]

    public class UserController : ControllerBase
    {

        private readonly BookingServiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public UserController(BookingServiceDbContext context, IMapper mapper, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                var userDto = _mapper.Map<UserDto>(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                var userDto = _mapper.Map<UserDto>(user);

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                if (user.Email != userUpdateDto.Email && await _context.Users.AnyAsync(u => u.Email == userUpdateDto.Email))
                {
                    return BadRequest(new { message = "Użytkownik z takim adresem Email już istnieje" });
                }

                _mapper.Map(userUpdateDto, user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Profil zaktualizowany" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserById(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var adminId = _userService.GetUserId(User);
                if (adminId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var admin = await _context.Users.FindAsync(adminId);
                if (admin == null || admin.Role != Roles.Admin)
                {
                    return Forbid();
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                if (user.Email != userUpdateDto.Email && await _context.Users.AnyAsync(u => u.Email == userUpdateDto.Email))
                {
                    return BadRequest(new { message = "Użytkownik z takim adresem Email już istnieje" });
                }

                _mapper.Map(userUpdateDto, user);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return Ok(new { message = "Profil zaktualizowany" });
                }

                return BadRequest(new { message = "Nie udało się zaktualizować profilu" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("bookings")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingListDto>>> GetUserBookings()
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var bookings = await _context.Bookings
                    .Include(b => b.Service)
                        .ThenInclude(s => s.Employee)
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                var bookingListDtos = _mapper.Map<List<BookingListDto>>(bookings);
                return Ok(bookingListDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPost("upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Nie udało się załadować pliku" });
                }

                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                var googleCloudStorageService = new GoogleCloudStorageService();
                var objectName = $"avatars/{userId}_{file.FileName}";

                using (var stream = file.OpenReadStream())
                {
                    var fileUrl = await googleCloudStorageService.UploadFileAsync(stream, objectName);

                    user.AvatarUrl = fileUrl;
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Dodano zdjęcie profilu", avatarUrl = user.AvatarUrl });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpDelete("delete-avatar")]
        [Authorize]
        public async Task<IActionResult> DeleteAvatar()
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                if (string.IsNullOrEmpty(user.AvatarUrl))
                {
                    return BadRequest(new { message = "Użytkownik nie ma przypisanego zdjęcia profilu" });
                }

                var avatarUrl = user.AvatarUrl;
                var objectName = avatarUrl.Split('/').Last();
                var googleCloudStorageService = new GoogleCloudStorageService();

                await googleCloudStorageService.DeleteFileAsync(objectName);

                user.AvatarUrl = null;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Zdjęcie profilu zostało usunięte" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("favorites")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteBusinesses()
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                var favoriteBusinesses = await _context.UserFavorites
                    .Where(uf => uf.UserId == userId)
                    .Include(uf => uf.Business.Images)
                    .Select(uf => uf.Business)
                    .ToListAsync();

                var favoriteBusinessesDtos = _mapper.Map<List<FavoriteBusinessDto>>(favoriteBusinesses);

                return Ok(favoriteBusinessesDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("favorites/{businessId}/exists")]
        [Authorize]
        public async Task<IActionResult> IsBusinessFavorite(int businessId)
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                var isFavorite = await _context.UserFavorites.AnyAsync(uf => uf.UserId == userId && uf.BusinessId == businessId);

                return Ok(isFavorite);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPost("favorites/{businessId}")]
        [Authorize]
        public async Task<IActionResult> AddToFavorites(int businessId)
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                var business = await _context.Businesses.FindAsync(businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var exitingFavorite = await _context.UserFavorites.AnyAsync(uf => uf.UserId == userId && uf.BusinessId == businessId);
                if (exitingFavorite)
                {
                    return BadRequest(new { message = "Ten biznes już dodany do ulubionych" });
                }

                var userFavorite = new UserFavorite
                {
                    UserId = userId.Value,
                    BusinessId = businessId,
                };

                _context.UserFavorites.Add(userFavorite);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Biznes dodany do ulubionych" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpDelete("favorites/{businessId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromFavorites(int businessId)
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                var userFavorite = await _context.UserFavorites.FirstOrDefaultAsync(uf => uf.UserId == userId && uf.BusinessId == businessId);

                if (userFavorite == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznes w ulubionych" });
                }

                _context.UserFavorites.Remove(userFavorite);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Biznes usunięty z ulubionych" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("reviews")]
        [Authorize]
        public async Task<IActionResult> GetUserReviews()
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var reviews = _context.Reviews.Where(r => r.UserId == userId).Include(r => r.Business).ToList();

                var reviewDtos = _mapper.Map<List<UserReviewDto>>(reviews);

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }
    }
}
