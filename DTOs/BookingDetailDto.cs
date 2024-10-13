namespace BookingService.DTOs
{
    public class BookingDetailDto
    {
        public int Id { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string EmployeeName { get; set; }
        public string ClientName { get; set; }
    }
}
