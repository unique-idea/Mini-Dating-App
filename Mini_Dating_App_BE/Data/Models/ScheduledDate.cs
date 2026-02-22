namespace Mini_Dating_App_BE.Data.Models
{
    public class ScheduledDate
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public Guid MatchId { get; set; }
        public Match? Match { get; set; }
    }
}
