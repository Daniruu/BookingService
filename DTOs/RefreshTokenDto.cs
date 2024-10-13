using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Access Token jest wymagany")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "Refresh Token jest wymagany")]
        public string RefreshToken { get; set; }
    }
}
