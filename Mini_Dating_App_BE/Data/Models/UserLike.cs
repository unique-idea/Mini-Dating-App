

namespace Mini_Dating_App_BE.Data.Models
{
    public class UserLike
    {
        public Guid UserLikeId { get; set; } = Guid.NewGuid();
        public Guid LikerId { get; set; }
        public User? Liker { get; set; } 


        public Guid LikedId { get; set; }
        public User? Liked { get; set; } 

    }
}
