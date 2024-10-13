using System.Security.Claims;
using BookingService.Data;
using BookingService.Models;

namespace BookingService.Services
{
    public class UserService
    {
        private readonly BookingServiceDbContext _context;

        public UserService(BookingServiceDbContext context)
        {
            _context = context;
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

        public async Task<bool> IsOwnerOrAdminAsync(int userId, int businessid)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (user.Role.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var business = await _context.Businesses.FindAsync(businessid);
            if (business == null)
            {
                return false;
            }

            return business.UserId == user.Id;
        }
    }
}
