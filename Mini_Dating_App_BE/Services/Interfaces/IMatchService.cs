using Mini_Dating_App_BE.DTOs.Responses;
using Mini_Dating_App_BE.DTOs.Requests;

namespace Mini_Dating_App_BE.Services.Interfaces
{
    public interface IMatchService
    {
        public Task<UserMatchesRes> GetUserMatches();
        public Task<ScheduleMatchRes> ScheduleMatch(Guid matchId);

    }
}
