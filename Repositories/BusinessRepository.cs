using BookingService.Data;
using BookingService.Models;

namespace BookingService.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly BookingServiceDbContext _context;

        public BusinessRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Business> GetBusinessById(int businessId)
        {
            return await _context.Businesses.FindAsync(businessId);
        }
    }
}
