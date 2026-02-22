namespace Mini_Dating_App_BE.DTOs.Responses
{
    public class UserRes
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;      
    }
}
