namespace BookingService.DTOs
{
    public class UserReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string BusinessName { get; set; }
        public string? BusinessImage { get; set; }
    }
}
