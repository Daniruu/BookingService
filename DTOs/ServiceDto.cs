namespace BookingService.DTOs
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public int OrderIndex { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public bool IsFeatured { get; set; }
        public string Group { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string? EmployeeAvatarUrl { get; set; }
    }
}
