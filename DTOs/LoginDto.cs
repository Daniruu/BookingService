using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string Password { get; set; }
    }
}
