namespace Mini_Dating_App_BE.DTOs.Responses
{
    public class ScheduleMatchRes
    {
        public Guid MatchId { get; set; }
        public string Status { get; set; } = string.Empty;
        public ScheduledDateRes? ScheduledDate { get; set; }
    }
}
