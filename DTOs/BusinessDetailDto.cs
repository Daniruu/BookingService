using BookingService.Models;

namespace BookingService.DTOs
{
    public class BusinessDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Nip { get; set; }
        public string Regon { get; set; }
        public string Krs { get; set; }
        public Address Address { get; set; }
        public IEnumerable<BusinessImageDto> Images { get; set; }
        public IEnumerable<WorkingHoursDto> WorkingHours { get; set; }
        public IEnumerable<EmployeeDto> Employees { get; set; }
        public IEnumerable<ServiceDto> Services { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
