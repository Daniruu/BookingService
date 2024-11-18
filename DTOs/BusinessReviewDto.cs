namespace BookingService.DTOs
{
    public class BusinessReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string UserName { get; set; }
        public string? UserAvatarUrl { get; set; }
    }
}
