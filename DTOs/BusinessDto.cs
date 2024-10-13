using BookingService.Models;

namespace BookingService.DTOs
{
    public class BusinessDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Nip { get; set; }
        public string Regon { get; set; }
        public string? Krs { get; set; }
        public Address Address { get; set; }
        public BusinessImageDto PrimaryImage { get; set; }
        public  bool IsPublished { get; set; }
    }
}
