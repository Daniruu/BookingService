namespace BookingService.Models
{
    public class Business
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Nip { get; set; }
        public string Regon { get; set; }
        public string? Krs { get; set; }
        public Address Address { get; set; }
        public List<WorkingHours> WorkingHours { get; set; }
        public List<Service> Services { get; set; }
        public List<Employee> Employees { get; set; }
        public List<BusinessImage> Images { get; set; }
        public bool IsPublished { get; set; } = false;
        public List<UserFavorite> UserFavorites { get; set; }
    }
}
