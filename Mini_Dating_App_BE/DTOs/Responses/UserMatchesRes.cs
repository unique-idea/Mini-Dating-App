namespace Mini_Dating_App_BE.DTOs.Responses
{
    public class UserMatchesRes
    {
        public bool HasAvailability { get; set; } = false;
        public List<UserMatchRes> Matches { get; set; } = null!;
    }
}
