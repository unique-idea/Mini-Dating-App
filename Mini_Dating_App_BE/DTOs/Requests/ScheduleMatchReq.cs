using System.ComponentModel.DataAnnotations;

namespace Mini_Dating_App_BE.DTOs.Requests
{
    public class ScheduleMatchReq
    {
        [Required]
        public Guid MatchId { get; set; }
    }
}
