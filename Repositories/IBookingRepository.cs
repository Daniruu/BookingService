using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetBookingsByUserId(int userId);
    }
}
