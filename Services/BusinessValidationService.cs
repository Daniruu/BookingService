using BookingService.Data;
using Microsoft.EntityFrameworkCore;

public class BusinessValidationService
{
    private readonly BookingServiceDbContext _context;

    public BusinessValidationService(BookingServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ValidateBusinessIsPublished(int businessId)
    {
        var business = await _context.Businesses
            .Include(b => b.Images)
            .Include(b => b.Employees)
            .Include(b => b.Services)
            .Include(b => b.WorkingHours)
            .FirstOrDefaultAsync(b => b.Id == businessId);

        if (business == null)
        {
            throw new ArgumentException("Nie znaleziono biznesu");
        }

        if (!business.Images.Any(i => i.IsPrimary))
        {
            return false;
        }

        if (!business.Employees.Any())
        {
            return false;
        }

        if (!business.Services.Any())
        {
            return false;
        }

        if (!business.WorkingHours.Any())
        {
            return false;
        }

        return true;
    }

    public async Task<bool> UnpublishBusinessAsync(int businessId)
    {
        var business = await _context.Businesses.FindAsync(businessId);
        if (business == null)
        {
            throw new ArgumentException("Nie znaleziono biznesu");
        }

        business.IsPublished = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
