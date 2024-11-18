namespace BookingService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } // 'admin', 'user', 'owner'
        public List<Booking> Bookings { get; set; }
        public List<UserFavorite> UserFavorites { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshExpiryTime { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
