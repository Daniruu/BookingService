namespace BookingService.DTOs
{
    public class WorkingHoursUpdateDto
    {
        public string DayOfWeek { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}
