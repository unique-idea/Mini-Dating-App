using AutoMapper;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.Repositories.Interfaces;
using Mini_Dating_App_BE.Services.Interfaces;

namespace Mini_Dating_App_BE.Services.Implements
{
    public class UserLikeService : BaseService<UserLikeService>, IUserLikeService
    {
        public UserLikeService(IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<UserLikeService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task LikeUserProfile(Guid likedId)
        {
            var currentUserId = GetUserId();

            await ValidateLiking(likedId, currentUserId);

            var newUserLike = new UserLike
            {
                LikedId = likedId,
                LikerId = GetUserId()
            };

            await _unitOfWork.GetRepository<UserLike>().InsertAsync(newUserLike);
            await _unitOfWork.CommitAsync();

            await CheckingMatch(likedId, currentUserId);
        }

        private async Task ValidateLiking(Guid likedId, Guid currentUserId)
        {
            if (likedId == currentUserId) throw new BadHttpRequestException("You cannot like your own profile.");

            var likedUserExist = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.UserId == likedId);
            if (likedUserExist == null) throw new KeyNotFoundException("Id of User not exist.");

            var likeExist = await _unitOfWork.GetRepository<UserLike>()
                .SingleOrDefaultAsync(predicate: x => x.LikerId == currentUserId && x.LikedId == likedId);

            if (likeExist != null) throw new BadHttpRequestException("You have already liked this profile.");
        }

        private async Task CheckingMatch(Guid likedId, Guid currentUserId)
        {
            var isMatch = await _unitOfWork.GetRepository<UserLike>()
                         .SingleOrDefaultAsync(predicate: x => x.LikerId == likedId && x.LikedId == currentUserId);

            if (isMatch != null)
            {
                var newMatch = new Match
                {
                    UserAId = currentUserId,
                    UserBId = likedId,
                    //MatchedAt = DateTime.UtcNow,  
                };

                await _unitOfWork.GetRepository<Match>().InsertAsync(newMatch);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
