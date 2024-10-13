namespace BookingService.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public Business Business { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public string? AvatarUrl { get; set; }
        public List<WorkingHours> WorkingHours { get; set; }
    }
}
