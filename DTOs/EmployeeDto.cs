using BookingService.Models;

namespace BookingService.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public string? AvatarUrl { get; set; }
        public List<WorkingHoursDto> WorkingHours { get; set; }
    }
}
