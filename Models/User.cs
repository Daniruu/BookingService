using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } // 'admin', 'user', 'owner'
        public List<Booking> Bookings { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshExpiryTime { get; set; }
    }
}
