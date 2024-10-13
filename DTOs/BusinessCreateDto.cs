using BookingService.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BookingService.DTOs
{
    public class BusinessCreateDto
    {
        [Required(ErrorMessage = "Nazwa bizneu jest wymagana")]
        [StringLength(50, ErrorMessage = "Nazwa nie może zawierać więcej niż 50 znaków")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Opis biznesu jest wymagany")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kategoria przedsiębiorstwa jest wymagana")]
        public string Category { get; set; }


        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany")]
        [Phone(ErrorMessage = "Nieprawidłowy format numeru telefonu")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Nip jest wymagany")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Nip musi składać się z 10 cyfr")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Nip musi składać się z 10 cyfr")]
        public string Nip { get; set; }

        [Required(ErrorMessage = "Regon jest wymagany")]
        [RegularExpression(@"^\d{9}$|^\d{14}$", ErrorMessage = "Regon musi mieć 9 lub 14 cyfr")]
        public string Regon { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Krs musi mieć 10 cyfr")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Krs musi składać się z 10 cyfr")]
        public string? Krs { get; set; }

        [Required(ErrorMessage = "Adres jest wymagany")]
        public AddressDto Address { get; set; }
    }
}
