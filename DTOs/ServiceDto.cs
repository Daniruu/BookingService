namespace BookingService.DTOs
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public bool IsFeatured { get; set; }
        public string Group { get; set; }
        public string EmployeeId { get; set; }
    }
}
