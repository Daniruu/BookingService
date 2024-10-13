namespace BookingService.Models
{
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string BuildingNumber { get; set; }
        public string? RoomNumber { get; set; }
    }
}
