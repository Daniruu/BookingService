using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class EmployeeCreateDto
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

        [Required(ErrorMessage = "Stanowisko jest wymagane")]
        public string Position { get; set; }
    }
}
