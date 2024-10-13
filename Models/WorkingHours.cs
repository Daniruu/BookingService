namespace BookingService.Models
{
    public class WorkingHours
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public int? BusinessId { get; set; }
        public Business Business { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
