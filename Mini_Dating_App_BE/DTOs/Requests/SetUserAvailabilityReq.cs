using System.ComponentModel.DataAnnotations;

namespace Mini_Dating_App_BE.DTOs.Requests
{
    public class SetUserAvailabilityReq
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }

    }
}
