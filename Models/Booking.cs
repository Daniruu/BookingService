namespace BookingService.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string Status { get; set; } // 'pending', 'canceled', 'completed'
        public DateTimeOffset CreatedAt { get; set; }
    }
}
