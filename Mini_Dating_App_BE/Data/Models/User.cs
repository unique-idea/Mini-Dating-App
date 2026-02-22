namespace Mini_Dating_App_BE.Data.Models
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;  
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public ICollection<UserLike> LikesGiven { get; set; } = new List<UserLike>();
        public ICollection<UserLike> LikesReceived { get; set; } = new List<UserLike>();
        public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();


    }
}
