namespace BookingService.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; } // duration in minutes
        public bool IsFeatured { get; set; }
        public string Group { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }
    }
}
