using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class ServiceCreateDto
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(100, ErrorMessage = "Imię nie może zawierać więcej niż 100 znaków")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany")]
        [StringLength(500, ErrorMessage = "Imię nie może zawierać więcej niż 500 znaków")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana")]
        [Range(0, double.MaxValue, ErrorMessage = "Cena musi być dodatnia")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Czas trwania jest wymagany")]
        [Range(1, 480, ErrorMessage = "Czas trwania musi być między 1 a 480 minut")]
        public int Duration { get; set; }

        [StringLength(50, ErrorMessage = "Nazwa grupy może zawierać maksymalnie 50 znaków.")]
        public string Group { get; set; }

        [Required(ErrorMessage = "Pracownik jest wymagany")]
        public int EmployeeId { get; set; }
    }
}
