using Microsoft.AspNetCore.Mvc;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using BookingService.Services;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/users")]

    public class UserController : ControllerBase
    {

        private readonly BookingServiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserController(BookingServiceDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.GetUser(userId.Value);

            if (!result.Success)
            {
                return NotFound(new { message = result.ErrorMessage });
            }

            return Ok(result.Data);
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.UpdateUser(userId.Value, dto);

            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Dane użytkownika zostały zaktualizowane.", user = result.Data });
        }

        [HttpPost("avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Nie udało się załadować pliku." });
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = "Plik jest za duży. Maksymalny rozmiar to 5 MB." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                return BadRequest(new { message = "Nieobsługiwany format pliku." });
            }

            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.UploadAvatar(userId.Value, file);

            if (!result.Success)
            {
                if (result.ErrorMessage == "Nie znaleziono użytkownika.")
                {
                    return NotFound(new { message = result.ErrorMessage });
                }

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Dodano zdjęcie profilu", avatarUrl = result.Data });
        }

        [HttpDelete("avatar")]
        [Authorize]
        public async Task<IActionResult> DeleteAvatar()
        {
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.DeleteAvatar(userId.Value);

            if (!result.Success)
            {
                if (result.ErrorMessage == "Nie znaleziono użytkownika.")
                {
                    return NotFound(new { message = result.ErrorMessage });
                }

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Zdjęcie profilu zostało usunięte." });
        }

        [HttpGet("bookings")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingListDto>>> GetUserBookings()
        {
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.GetUserBookings(userId.Value);

            if (!result.Success)
            {
                if (result.ErrorMessage == "Nie znaleziono użytkownika.")
                {
                    return NotFound(new { message = result.ErrorMessage });
                }

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(result.Data);
        }
    }
}
