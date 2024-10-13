using BookingService.Models;

namespace BookingService.DTOs
{
    public class BookingListDto
    {
        public int Id { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string Status { get; set; }
        public string ServiceName { get; set; }
        public string EmployeeName { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }
}
