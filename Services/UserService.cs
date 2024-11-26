using System.Security.Claims;
using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;

        public UserService(
            IUserRepository userRepository, 
            IBusinessRepository businessRepository, 
            IBookingRepository bookingRepository,
            IMapper mapper, 
            IGoogleCloudStorageService googleCloudStorageService)
        {
            _userRepository = userRepository;
            _businessRepository = businessRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _googleCloudStorageService = googleCloudStorageService;
        }

        public int? GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return null;
            }

            return int.Parse(userIdClaim);
        }

        public async Task<ServiceResult<UserDto>> GetUser(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<UserDto>.Failure("Nie znaleziono użytkownika.");
            }

            var userDto = _mapper.Map<UserDto>(user);
            
            return ServiceResult<UserDto>.SuccessResult(userDto);
        }

        public async Task<ServiceResult<UserDto>> UpdateUser(int userId, UserUpdateDto updateUserDto)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<UserDto>.Failure("Nie znaleziono użytkownika.");
            }

            if (user.Email != updateUserDto.Email)
            {
                if (await _userRepository.UserExistsAsync(updateUserDto.Email))
                {
                    return ServiceResult<UserDto>.Failure("Użytkownik z takim adresem Email już istnieje.");
                }
            }

            _mapper.Map(updateUserDto, user);

            await _userRepository.UpdateUserAsync(user);

            var userDto = _mapper.Map<UserDto>(user);

            return ServiceResult<UserDto>.SuccessResult(userDto);
        }

        public async Task<bool> IsOwnerOrAdminAsync(int userId, int businessId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return false;
            }

            if (user.Role.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var business = await _businessRepository.GetBusinessById(businessId);
            if (business == null)
            {
                return false;
            }

            return business.UserId == user.Id;
        }

        public async Task<ServiceResult<string>> UploadAvatar(int userId, IFormFile file)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<string>.Failure("Nie znaleziono użytkownika.");
            }

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var existingObjectName = user.AvatarUrl.Split('/').Last();

                try
                {
                    await _googleCloudStorageService.DeleteFileAsync(existingObjectName);
                }
                catch (Exception ex)
                {
                    return ServiceResult<string>.Failure($"Błąd podczas usuwania starego pliku: {ex.Message}");
                }
            }

            var objectName = $"avatars/{userId}_{Path.GetRandomFileName()}{Path.GetExtension(file.FileName)}";

            using (var stream = file.OpenReadStream())
            {
                try
                {
                    var fileUrl = await _googleCloudStorageService.UploadFileAsync(stream, objectName);

                    user.AvatarUrl = fileUrl;
                    await _userRepository.UpdateUserAsync(user);

                    return ServiceResult<string>.SuccessResult(fileUrl);
                }
                catch (Exception ex)
                {
                    return ServiceResult<string>.Failure($"Błąd podczas zapisywania pliku: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResult> DeleteAvatar(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<string>.Failure("Nie znaleziono użytkownika.");
            }

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var existingObjectName = user.AvatarUrl.Split('/').Last();

                try
                {
                    await _googleCloudStorageService.DeleteFileAsync(existingObjectName);
                }
                catch (Exception ex)
                {
                    return ServiceResult<string>.Failure($"Błąd podczas usuwania pliku: {ex.Message}");
                }
            }

            user.AvatarUrl = null;
            await _userRepository.UpdateUserAsync(user);

            return ServiceResult.SuccessResult();
        }

        public async Task<ServiceResult<List<BookingListDto>>> GetUserBookings(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<List<BookingListDto>>.Failure("Nie znaleziono użytkownika.");
            }

            var bookings = await _bookingRepository.GetBookingsByUserId(userId);

            var bookingListDtos = _mapper.Map<List<BookingListDto>>(bookings);

            return ServiceResult <List<BookingListDto>>.SuccessResult(bookingListDtos);
        }
    }
}
