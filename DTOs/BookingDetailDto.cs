namespace BookingService.DTOs
{
    public class BookingDetailDto
    {
        public int Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Status { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessEmail { get; set; }
        public AddressDto BusinessAddress { get; set; }
        public string EmployeeName { get; set; }
        public string? EmployeeAvatarUrl { get; set; }
        public string ClientName { get; set; }
        public BusinessImageDto BusinessImage { get; set; }
    }
}
