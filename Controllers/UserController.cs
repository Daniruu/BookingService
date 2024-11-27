using Microsoft.AspNetCore.Mvc;
using BookingService.DTOs;
using BookingService.Models;
using Microsoft.AspNetCore.Authorization;
using BookingService.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/users")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
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
                return result.ErrorMessage switch
                {
                    "Nie znaleziono użytkownika." => NotFound(new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage })
                };
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
                return result.ErrorMessage switch
                {
                    "Nie znaleziono użytkownika." => NotFound(new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage })
                };
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
                return result.ErrorMessage switch
                {
                    "Nie znaleziono użytkownika." => NotFound(new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage })
                };
            }

            return Ok(new { message = result.Data });
        }

        [HttpGet("bookings")]
        [Authorize]
        public async Task<ActionResult<List<BookingListDto>>> GetUserBookings()
        {
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.GetUserBookings(userId.Value);

            if (!result.Success)
            {
                return result.ErrorMessage switch
                {
                    "Nie znaleziono użytkownika." => NotFound(new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("favorites")]
        [Authorize]
        public async Task<IActionResult> GetUserFavorites()
        {
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.GetUserFavorites(userId.Value);

            if (!result.Success)
            {
                return result.ErrorMessage switch
                {
                    "Nie znaleziono użytkownika." => NotFound(new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }

        [HttpPatch("favorites")]
        [Authorize]
        public async Task<IActionResult> UpdateUserFavorite([FromBody] UpdateFavoriteDto dto)
        {
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.UpdateUserFavorites(userId.Value, dto.BusinessId, dto.IsFavorite);

            if (!result.Success)
            {
                return result.ErrorMessage switch
                {
                    "Nie znaleziono użytkownika." => NotFound(new { message = result.ErrorMessage }),
                    "Nie znaleziono firmy." => NotFound(new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage })
                };
            }

            return Ok(new { message = result.Data });
        }

        [HttpGet("favorites/{businessId}/exists")]
        [Authorize]
        public async Task<IActionResult> IsBusinessFavorite(int businessId)
        {
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.IsBusinessFavorite(userId.Value, businessId);

            if (!result.Success)
            {
                return result.ErrorMessage switch
                {
                    "Nie znaleziono użytkownika." => NotFound(new { message = result.ErrorMessage }),
                    "Nie znaleziono firmy." => NotFound(new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }

        
        [HttpGet("reviews")]
        [Authorize]
        public async Task<IActionResult> GetUserReviews()
        {
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Użytkownik nieautoryzowany." });
            }

            var result = await _userService.GetUserReviews(userId.Value);

            if (!result.Success)
            {
                return result.ErrorMessage switch
                {
                    "Nie znaleziono użytkownika." => NotFound(new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }
    }
}
