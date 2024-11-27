using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly BookingServiceDbContext _context;

        public ReviewRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetReviewsByUserId(int userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .Include(r => r.Business)
                    .ThenInclude(b => b.Images)
                .ToListAsync();
        }
    }
}
