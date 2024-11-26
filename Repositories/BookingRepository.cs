using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingServiceDbContext _context;

        public BookingRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetBookingsByUserId(int userId)
        {
            return await _context.Bookings.Include(b => b.Service).ThenInclude(s => s.Employee).Where(b => b.UserId == userId).ToListAsync();
        }
    }
}
