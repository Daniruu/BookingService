using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IBusinessRepository
    {
        Task<Business> GetBusinessById(int businessId);
    }
}
