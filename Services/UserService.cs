using System.Security.Claims;
using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;

namespace BookingService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository, 
            IBusinessRepository businessRepository, 
            IBookingRepository bookingRepository,
            IReviewRepository reviewRepository,
            IMapper mapper, 
            IGoogleCloudStorageService googleCloudStorageService)
        {
            _userRepository = userRepository;
            _businessRepository = businessRepository;
            _bookingRepository = bookingRepository;
            _reviewRepository = reviewRepository;
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
                var existingFileName = user.AvatarUrl.Split('/').Last();

                try
                {
                    await _googleCloudStorageService.DeleteFileAsync("avatars", existingFileName);
                }
                catch (Exception ex)
                {
                    return ServiceResult<string>.Failure($"Błąd podczas usuwania starego pliku: {ex.Message}");
                }
            }

            var fileName = $"{userId}_{Path.GetRandomFileName()}{Path.GetExtension(file.FileName)}";

            using (var stream = file.OpenReadStream())
            {
                try
                {
                    var fileUrl = await _googleCloudStorageService.UploadFileAsync(stream, "avatars", fileName);

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

        public async Task<ServiceResult<string>> DeleteAvatar(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<string>.Failure("Nie znaleziono użytkownika.");
            }

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var existingFileName = user.AvatarUrl.Split('/').Last();

                try
                {
                    await _googleCloudStorageService.DeleteFileAsync("avatars", existingFileName);
                }
                catch (Exception ex)
                {
                    return ServiceResult<string>.Failure($"Błąd podczas usuwania pliku: {ex.Message}");
                }
            }

            user.AvatarUrl = null;
            await _userRepository.UpdateUserAsync(user);

            return ServiceResult<string>.SuccessResult("Zdjęcie profilu zostało usunięte.");
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

        public async Task<ServiceResult<List<FavoriteBusinessDto>>> GetUserFavorites(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<List<FavoriteBusinessDto>>.Failure("Nie znaleziono użytkownika.");
            }

            var favoriteBusinesses = await _userRepository.GetUserFavoriteBusinesses(userId);

            var favoriteBusinessesDtos = _mapper.Map<List<FavoriteBusinessDto>>(favoriteBusinesses);

            return ServiceResult<List<FavoriteBusinessDto>>.SuccessResult(favoriteBusinessesDtos);
        }

        public async Task<ServiceResult<string>> UpdateUserFavorites(int userId, int businessId, bool isFavorite)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<string>.Failure("Nie znaleziono użytkownika.");
            }

            var business = await _businessRepository.GetBusinessById(businessId);
            if (business == null)
            {
                return ServiceResult<string>.Failure("Nie znaleziono firmy.");
            }

            if (isFavorite)
            {
                var alreadyFavorite = await _userRepository.IsBusinessFavorite(userId, businessId);
                if (alreadyFavorite)
                {
                    return ServiceResult<string>.Failure("Firma już znajduje się w ulubionych.");
                }

                await _userRepository.AddToFavorites(userId, businessId);
                return ServiceResult<string>.SuccessResult("Dodano firmę do ulubionych.");
            }
            else
            {
                var isFavoriteBusiness = await _userRepository.IsBusinessFavorite(userId, businessId);
                if (!isFavoriteBusiness)
                {
                    return ServiceResult<string>.Failure("Firma nie znajduje się w ulubionych.");
                }

                await _userRepository.RomoveFromFavorites(userId, businessId);
                return ServiceResult<string>.SuccessResult("Usunięto firmę z ulubionych.");
            }
        }

        public async Task<ServiceResult<bool>> IsBusinessFavorite(int userId, int businessId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<bool>.Failure("Nie znaleziono użytkownika.");
            }

            var business = await _businessRepository.GetBusinessById(businessId);
            if (business == null)
            {
                return ServiceResult<bool>.Failure("Nie znaleziono firmy.");
            }

            var isFavoriteBusiness = await _userRepository.IsBusinessFavorite(userId, businessId);

            return ServiceResult<bool>.SuccessResult(isFavoriteBusiness);
        }

        public async Task<ServiceResult<List<UserReviewDto>>> GetUserReviews(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<List<UserReviewDto>>.Failure("Nie znaleziono użytkownika.");
            }

            var reviews = await _reviewRepository.GetReviewsByUserId(userId);

            var reviewDtos = _mapper.Map<List<UserReviewDto>>(reviews);

            return ServiceResult<List<UserReviewDto>>.SuccessResult(reviewDtos);
        }
    }
}
