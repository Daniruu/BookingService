﻿using BookingService.DTOs;
using BookingService.Utils;
using System.Security.Claims;

namespace BookingService.Services
{
    public interface IUserService
    {
        int? GetUserId(ClaimsPrincipal user);
        Task<ServiceResult<UserDto>> GetUser(int userId);
        Task<ServiceResult<UserDto>> UpdateUser(int userId, UserUpdateDto dto);
        Task<bool> IsOwnerOrAdminAsync(int userId, int businessId);
        Task<ServiceResult<string>> UploadAvatar(int userId, IFormFile file);
        Task<ServiceResult<string>> DeleteAvatar(int userId);
        Task<ServiceResult<List<BookingListDto>>> GetUserBookings(int userId);
        Task<ServiceResult<List<FavoriteBusinessDto>>> GetUserFavorites(int userId);
        Task<ServiceResult<string>> UpdateUserFavorites(int userId, int businessId, bool isFavorite);
        Task<ServiceResult<bool>> IsBusinessFavorite(int userId, int businessId);
        Task<ServiceResult<List<UserReviewDto>>> GetUserReviews(int userId);
    }
}
