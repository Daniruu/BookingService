using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50, ErrorMessage = "Nazwa nie może zawierać więcej niż 50 znaków")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany")]
        [Phone(ErrorMessage = "Nieprawidłowy format numeru telefonu")]
        public string Phone { get; set; }
    }
}
