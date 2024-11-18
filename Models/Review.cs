namespace BookingService.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int BusinessId { get; set; }
        public Business Business { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
