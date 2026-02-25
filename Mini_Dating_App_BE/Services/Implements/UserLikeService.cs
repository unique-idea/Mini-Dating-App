using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.Hubs;
using Mini_Dating_App_BE.Repositories.Interfaces;
using Mini_Dating_App_BE.Services.Interfaces;

namespace Mini_Dating_App_BE.Services.Implements
{
    public class UserLikeService : BaseService<UserLikeService>, IUserLikeService
    {
        public UserLikeService(IHubContext<SystemHub> hubContext, IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<UserLikeService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(hubContext, unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<bool> LikeUserProfile(Guid likedId)
        {
            var currentUserId = GetUserId();

            await ValidateLikeEligibility(likedId, currentUserId);

            await CreateUserLike(likedId, currentUserId);

            var mutualLike = await _unitOfWork.GetRepository<UserLike>()
             .SingleOrDefaultAsync(predicate: x => x.LikerId == likedId && x.LikedId == currentUserId);

            if (mutualLike != null)
            {
                await CreateMatchAndNotifyUser(currentUserId, likedId);
                return true;
            }

            await _unitOfWork.CommitAsync();

            return true;
        }

        private async Task CreateUserLike(Guid likedId, Guid currentUserId)
        {
            var newUserLike = new UserLike { LikedId = likedId, LikerId = currentUserId };

            await _unitOfWork.GetRepository<UserLike>().InsertAsync(newUserLike);
        }

        private async Task ValidateLikeEligibility(Guid likedId, Guid currentUserId)
        {
            if (likedId == currentUserId) throw new BadHttpRequestException("You cannot like your own profile.");

            var likedUserExist = await _unitOfWork.GetRepository<User>()
                .SingleOrDefaultAsync(predicate: x => x.UserId == likedId, include: x => x.Include(x => x.LikesReceived));

            if (likedUserExist == null) throw new KeyNotFoundException("Id of Liked User not exist.");

            if (likedUserExist.LikesReceived.Any(lr => lr.LikerId == currentUserId)) throw new BadHttpRequestException("You have already liked this profile.");

        }

        private async Task CreateMatchAndNotifyUser(Guid userAId, Guid userBId)
        {
            var newMatch = new Match { UserAId = userAId, UserBId = userBId };
            await _unitOfWork.GetRepository<Match>().InsertAsync(newMatch);
            await _unitOfWork.CommitAsync();

            await NotifyUsers(new[] { userAId, userBId }, "matchcreated", new
            {
                matchId = newMatch.MatchId,
            });
        }
    }
}
