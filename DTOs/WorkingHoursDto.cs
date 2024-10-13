namespace BookingService.DTOs
{
    public class WorkingHoursDto
    {
        public string DayOfWeek { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}
