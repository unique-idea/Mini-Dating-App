namespace Mini_Dating_App_BE.DTOs.Responses
{
    public class AuthenticationRes
    {
        public string Token { get; set; } = string.Empty;
        public UserRes User { get; set; } = null!;
    }
}
