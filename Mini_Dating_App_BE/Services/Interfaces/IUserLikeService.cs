using Mini_Dating_App_BE.DTOs.Requests;

namespace Mini_Dating_App_BE.Services.Interfaces
{
    public interface IUserLikeService
    {
        Task<bool> LikeUserProfile(Guid likedId);
    }
}
