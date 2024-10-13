namespace BookingService.Models
{
    public class BusinessImage
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; } = false;

        public Business Business { get; set; }
    }
}
