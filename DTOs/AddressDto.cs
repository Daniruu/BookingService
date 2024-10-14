using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class AddressDto
    {
        [Required(ErrorMessage = "Ulica jest wymagana")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Miasto jest wymagane")]
        public string City { get; set; }

        [Required(ErrorMessage = "Wojewódzdwo jest wymagane")]
        public string Region { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Numer domu jest wymagany")]
        public string BuildingNumber { get; set; }

        public string? RoomNumber { get; set; }
    }
}
