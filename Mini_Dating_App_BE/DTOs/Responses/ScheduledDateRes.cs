namespace Mini_Dating_App_BE.DTOs.Responses
{
    public class ScheduledDateRes
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
