using AutoMapper;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.Enums;
using Mini_Dating_App_BE.DTOs.Responses;
using Mini_Dating_App_BE.Repositories.Interfaces;
using Mini_Dating_App_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mini_Dating_App_BE.Services.Implements
{
    public class MatchService : BaseService<MatchService>, IMatchService
    {
        public MatchService(IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<MatchService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        #region GetMatches
        public async Task<UserMatchesRes> GetUserMatches()
        {
            var currentUserId = GetUserId();

            var matches = await GetMatches(currentUserId);

            var matchesResponse = await BuildMatchesResponse(matches, currentUserId);

            var hasAvailability = await HasAvailability(currentUserId);

            return new UserMatchesRes
            {
                Matches = matchesResponse,
                HasAvailability = hasAvailability
            };
        }


        private async Task<List<Match>> GetMatches(Guid userId)
        {
            return (await _unitOfWork.GetRepository<Match>().
                   GetListAsync(predicate: x => x.UserAId == userId || x.UserBId == userId,
                   include: x => x.Include(x => x.ScheduledDate!))).ToList();
        }

        private async Task<List<UserMatchRes>> BuildMatchesResponse(List<Match> matches, Guid currentUserId)
        {
            if (!matches.Any()) return new List<UserMatchRes>();

            var matchedUserIds = matches
                .Select(m => m.UserAId == currentUserId ? m.UserBId : m.UserAId)
                .Distinct()
                .ToList();

            var matchedUsers = await _unitOfWork
                .GetRepository<User>()
                .GetListAsync(predicate: x => matchedUserIds.Contains(x.UserId));

            var matchedUserDict = matchedUsers.ToDictionary(x => x.UserId);

            return matches.Select(m =>
            {
                var matchedUserId = m.UserAId == currentUserId
                    ? m.UserBId
                    : m.UserAId;

                var response = _mapper.Map<UserMatchRes>(m);

                if (matchedUserDict.TryGetValue(matchedUserId, out var user)) response.User = _mapper.Map<UserRes>(user);

                if (m.ScheduledDate != null) response.ScheduledDate = _mapper.Map<ScheduledDateRes>(m.ScheduledDate);

                return response;

            }).ToList();
        }

        #endregion
        #region ScheduleMatch
        public async Task<ScheduleMatchRes> ScheduleMatch(Guid matchId)
        {
            var currentUserId = GetUserId();

            var match = await GetValidMatch(matchId, currentUserId);

            if (!await HasAvailability(currentUserId)) throw new BadHttpRequestException("You don't have any availability");

            ConfirmUser(match, currentUserId);

            if (IsOtherUserConfirmed(match, currentUserId))
            {
                var scheduled = await TryCreateSchedule(match, currentUserId);

                match.Status = scheduled
                    ? MatchStatusEnum.Scheduled
                    : MatchStatusEnum.Rescheduled;
            }
            else
            {
                match.Status = MatchStatusEnum.Scheduling;
            }

            await SaveMatch(match);

            return MapResponse(match);
        }

        private async Task<Match> GetValidMatch(Guid matchId, Guid currentUserId)
        {
            var match = await _unitOfWork.GetRepository<Match>().SingleOrDefaultAsync(predicate: x => x.MatchId == matchId);

            if (match == null) throw new KeyNotFoundException("Match not found");

            if (match.UserAId != currentUserId && match.UserBId != currentUserId) throw new BadHttpRequestException("You are not part of this match");

            if (match.ScheduledDate != null) throw new BadHttpRequestException("Match already scheduled");

            return match;
        }

        private void ConfirmUser(Match match, Guid userId)
        {
            if (match.UserAId == userId)
                match.UserAConfirmed = true;
            else
                match.UserBConfirmed = true;
        }

        private bool IsOtherUserConfirmed(Match match, Guid currentUserId)
        {
            return match.UserAId == currentUserId
                ? match.UserBConfirmed
                : match.UserAConfirmed;
        }

        private async Task<bool> TryCreateSchedule(Match match, Guid currentUserId)
        {
            var otherUserId = match.UserAId == currentUserId
                ? match.UserBId
                : match.UserAId;

            var repo = _unitOfWork.GetRepository<Availability>();

            var currentUserAvailabilities = (await repo.GetListAsync(predicate: x => x.UserId == currentUserId)).ToList();
            var otherUserAvailabilities = (await repo.GetListAsync(predicate: x => x.UserId == otherUserId)).ToList();

            if (!currentUserAvailabilities.Any() || !otherUserAvailabilities.Any()) return false;

            var minDateA = currentUserAvailabilities.Min(x => x.Date);
            var maxDateA = currentUserAvailabilities.Max(x => x.Date);

            var minDateB = otherUserAvailabilities.Min(x => x.Date);
            var maxDateB = otherUserAvailabilities.Max(x => x.Date);

            if (!IsDateRangeOverlapping(minDateA, maxDateA, minDateB, maxDateB)) return false;

            return await TryMatchTimeSlot(match, currentUserAvailabilities, otherUserAvailabilities, currentUserId);
        }

        private async Task<bool> TryMatchTimeSlot(Match match, List<Availability> current, List<Availability> other, Guid currentUserId)
        {
            var existingScheduledMatches = await _unitOfWork.GetRepository<Match>()
                .GetListAsync(predicate: x => (x.UserAId == currentUserId || x.UserBId == currentUserId)
                 && x.MatchId != match.MatchId
                 && x.ScheduledDate != null);

            var scheduledByDate = existingScheduledMatches
                .GroupBy(x => x.ScheduledDate!.Date.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            var otherByDate = other
                .GroupBy(x => x.Date.Date)
               .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var c in current)
            {
                if (!otherByDate.TryGetValue(c.Date.Date, out var sameDays)) continue;

                foreach (var sameDay in sameDays)
                {
                    if (scheduledByDate.TryGetValue(c.Date.Date, out var scheduledMatches)) continue;

                    if (!IsSlotRangeOverlapping(c, sameDay)) continue;

                    match.ScheduledDate = CreateScheduledDate(c, sameDay);
                    return true;
                }
            }

            return false;
        }

        private bool IsDateRangeOverlapping(DateTime minA, DateTime maxA, DateTime minB, DateTime maxB)
        {
            return minA <= maxB && minB <= maxA;
        }

        private bool IsSlotRangeOverlapping(Availability a, Availability b)
        {
            return a.StartTime < b.EndTime && b.StartTime < a.EndTime;
        }
        private ScheduledDate CreateScheduledDate(Availability a, Availability b)
        {
            return new ScheduledDate
            {
                Date = a.Date.Date,
                StartTime = a.StartTime > b.StartTime ? a.StartTime : b.StartTime,
                EndTime = a.EndTime < b.EndTime ? a.EndTime : b.EndTime
            };
        }
        private async Task SaveMatch(Match match)
        {
            _unitOfWork.GetRepository<Match>().Update(match);
            await _unitOfWork.CommitAsync();
        }

        private ScheduleMatchRes MapResponse(Match match)
        {
            var response = _mapper.Map<ScheduleMatchRes>(match);

            if (match.ScheduledDate != null)
            {
                response.ScheduledDate = _mapper.Map<ScheduledDateRes>(match.ScheduledDate);
            }

            return response;
        }
        #endregion
        private async Task<bool> HasAvailability(Guid userId)
        {
            return (await _unitOfWork.GetRepository<Availability>().GetListAsync(predicate: x => x.UserId == userId)).Any();
        }
    }
}
