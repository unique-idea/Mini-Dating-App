using Mini_Dating_App_BE.Enums;

namespace Mini_Dating_App_BE.DTOs.Responses
{
    public class UserMatchRes
    {
        public Guid MatchId { get; set; }
        public string Status { get; set; } = string.Empty;

        public UserRes User { get; set; } = null!;
        public ScheduledDateRes? ScheduledDate { get; set; }
    }
}

