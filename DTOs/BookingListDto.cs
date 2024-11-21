using BookingService.Models;

namespace BookingService.DTOs
{
    public class BookingListDto
    {
        public int Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string Status { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int ServiceDuration { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }
}
