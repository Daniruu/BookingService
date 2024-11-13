namespace BookingService.Models
{
    public class UserFavorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BusinessId { get; set; }

        public User User { get; set; }
        public Business Business { get; set; }
    }
}
