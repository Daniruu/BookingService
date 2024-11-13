using BookingService.Models;

namespace BookingService.DTOs
{
    public class FavoriteBusinessDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public Address Address { get; set; }
        public BusinessImageDto PrimaryImage { get; set; }
    }
}
