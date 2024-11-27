using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IReviewRepository
    {
        Task <List<Review>> GetReviewsByUserId(int userId);
    }
}
