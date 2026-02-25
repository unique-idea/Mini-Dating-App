namespace Mini_Dating_App_BE.DTOs.Responses
{
    public class LoginRes
    {
        public string Token { get; set; } = string.Empty;
        public UserRes User { get; set; } = null!;
    }
}
