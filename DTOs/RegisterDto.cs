using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50, ErrorMessage = "Imię nie może zawierać więcej niż 50 znaków")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany")]
        [Phone(ErrorMessage = "Nieprawidłowy format numeru telefonu")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi zawierać od 6 do 100 znaków")]
        public string Password { get; set; }
    }
}
