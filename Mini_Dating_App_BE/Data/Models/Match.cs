using Mini_Dating_App_BE.Enums;

namespace Mini_Dating_App_BE.Data.Models
{
    public class Match
    {
        public Guid MatchId { get; set; } = Guid.NewGuid();

        public Guid UserAId { get; set; }
        public User? UserA { get; set; }
        public bool UserAConfirmed { get; set; } = false;
        
        public Guid UserBId { get; set; }
        public User? UserB { get; set; }
        public bool UserBConfirmed { get; set; } = false;
        public MatchStatusEnum Status { get; set; } = MatchStatusEnum.Matched;

        public ScheduledDate? ScheduledDate { get; set; }
    }
}
