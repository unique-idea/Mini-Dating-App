namespace Mini_Dating_App_BE.Data.Models
{
    public class Availability
    {
        public Guid AvailabilityId { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; } 
    }
}
