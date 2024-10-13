using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class BookingCreateDto
    {
        [Required]
        public int ServiceId { get; set; }

        [Required]
        public DateTimeOffset DateTime { get; set; }
    }
}
