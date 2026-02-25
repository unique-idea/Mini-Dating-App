using AutoMapper;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.Enums;
using Mini_Dating_App_BE.DTOs.Responses;
using Mini_Dating_App_BE.Repositories.Interfaces;
using Mini_Dating_App_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Mini_Dating_App_BE.Hubs;

namespace Mini_Dating_App_BE.Services.Implements
{
    public class MatchService : BaseService<MatchService>, IMatchService
    {
        public MatchService(IHubContext<SystemHub> hubContext, IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<MatchService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(hubContext, unitOfWork, logger, mapper, httpContextAccessor)
        {
        }


        #region GetMatches
        public async Task<UserMatchesRes> GetUserMatches()
        {
            var currentUserId = GetUserId();

            var matches = await GetMatches(currentUserId);

            var matchesResponse = await BuildMatchesResponse(matches, currentUserId);

            var hasAvailability = await HasAvailability(currentUserId);

            return new UserMatchesRes { Matches = matchesResponse, HasAvailability = hasAvailability };
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
                var (matchedUserId, matchedUserConfrimed) = (m.UserAId == currentUserId)
                ? (m.UserBId, m.UserBConfirmed)
                : (m.UserAId, m.UserAConfirmed);

                var response = _mapper.Map<UserMatchRes>(m);

                if (matchedUserDict.TryGetValue(matchedUserId, out var user)) response.User = _mapper.Map<UserRes>(user);

                if (m.ScheduledDate != null) response.ScheduledDate = _mapper.Map<ScheduledDateRes>(m.ScheduledDate);

                response.UserMatchedConfirmed = matchedUserConfrimed;

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
                    : MatchStatusEnum.NoSlotFound;
            }
            else
            {
                match.Status = MatchStatusEnum.Pending;
            }

            await UpdateMatchAndNotifyUser(match);

            return MapResponse(match);
        }

        private async Task<Match> GetValidMatch(Guid matchId, Guid currentUserId)
        {
            var match = await _unitOfWork.GetRepository<Match>().SingleOrDefaultAsync(
                predicate: x => x.MatchId == matchId, include: x => x.Include(x => x.UserA).Include(x => x.UserB)!.Include(x => x.ScheduledDate)!);

            if (match == null) throw new KeyNotFoundException("Match not found");

            if (match.UserAId != currentUserId && match.UserBId != currentUserId) throw new BadHttpRequestException("You are not part of this match");

            var userMatched = match.UserAId == currentUserId ? match.UserB : match.UserA;

            if (match.Status == MatchStatusEnum.Pending && (match.UserAId == currentUserId ? match.UserAConfirmed : match.UserBConfirmed))
                throw new BadHttpRequestException("Date with " + userMatched!.Name + " still now try on scheduling please patient");

            if (match.Status == MatchStatusEnum.Scheduled)
                throw new BadHttpRequestException("Date with " + userMatched!.Name + " already scheduled at " + match.ScheduledDate!.Date.Day);

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

            var currentUserAvailabilities = (await repo.GetListAsync(predicate: x => x.UserId == currentUserId)).OrderBy(x => x.Date.Date).ToList();
            var otherUserAvailabilities = (await repo.GetListAsync(predicate: x => x.UserId == otherUserId)).OrderBy(x => x.Date.Date).ToList();

            if (!IsDateRangeOverLapping(currentUserAvailabilities, otherUserAvailabilities)) return false;

            return await TryMatchTimeSlot(match, currentUserAvailabilities, otherUserAvailabilities, currentUserId, otherUserId);
        }

        private bool IsDateRangeOverLapping(List<Availability> currents, List<Availability> others)
        {
            if (!currents.Any() || !others.Any()) return false;

            var minDateA = currents.Min(x => x.Date);
            var maxDateA = currents.Max(x => x.Date);

            var minDateB = others.Min(x => x.Date);
            var maxDateB = others.Max(x => x.Date);

            return minDateA <= maxDateB && minDateB <= maxDateA;
        }
        private async Task<bool> TryMatchTimeSlot(Match match, List<Availability> curAva, List<Availability> othAva, Guid currentUserId, Guid otherUserId)
        {
            var curBookedMatches = await _unitOfWork.GetRepository<Match>()
                .GetListAsync(predicate: x => (x.UserAId == currentUserId || x.UserBId == currentUserId)
                && x.MatchId != match.MatchId
                && x.Status == MatchStatusEnum.Scheduled,
                include: x => x.Include(x => x.ScheduledDate)!);

            var othBookedMathches = await _unitOfWork.GetRepository<Match>()
                .GetListAsync(predicate: x => (x.UserAId == otherUserId || x.UserBId == otherUserId)
                && x.MatchId != match.MatchId
                && x.Status == MatchStatusEnum.Scheduled,
                include: x => x.Include(x => x.ScheduledDate)!);

            var othAvaByDate = othAva
               .GroupBy(x => x.Date.Date)
               .ToDictionary(g => g.Key, g => g.First());

            foreach (var c in curAva)
            {
                if (!othAvaByDate.TryGetValue(c.Date.Date, out var othAvaSameDay)) continue;

                var potentialStartTime = c.StartTime > othAvaSameDay.StartTime ? c.StartTime : othAvaSameDay.StartTime;
                var potentialEndTime = c.EndTime < othAvaSameDay.EndTime ? c.EndTime : othAvaSameDay.EndTime;

                if (potentialStartTime >= potentialEndTime || (potentialEndTime - potentialStartTime).TotalMinutes < 30) continue;

                var hasConflict = curBookedMatches.Any(x => x.ScheduledDate!.Date.Date == c.Date.Date && x.ScheduledDate.StartTime < potentialEndTime &&
                                  x.ScheduledDate.EndTime > potentialStartTime) || othBookedMathches.Any(x => x.ScheduledDate!.Date.Date == c.Date.Date &&
                                  x.ScheduledDate.StartTime < potentialEndTime && x.ScheduledDate.EndTime > potentialStartTime);

                if (hasConflict) continue;

                match.ScheduledDate = new ScheduledDate { Date = c.Date.Date, StartTime = potentialStartTime, EndTime = potentialEndTime };

                return true;
            }


            return false;
        }

        private async Task UpdateMatchAndNotifyUser(Match match)
        {
            _unitOfWork.GetRepository<Match>().Update(match);
            await _unitOfWork.CommitAsync();


            await NotifyUser(match.UserAId, "matchupdated", new
            {
                matchId = match.MatchId,
                status = match.Status.ToString(),
                scheduledDate = match.ScheduledDate == null ? null : new
                {
                    date = match.ScheduledDate.Date,
                    startTime = match.ScheduledDate.StartTime,
                    endTime = match.ScheduledDate.EndTime
                },
                userMatchedConfirmed = match.UserBConfirmed
            });

            await NotifyUser(match.UserBId, "matchupdated", new
            {
                matchId = match.MatchId,
                status = match.Status.ToString(),
                scheduledDate = match.ScheduledDate == null ? null : new
                {
                    date = match.ScheduledDate.Date,
                    startTime = match.ScheduledDate.StartTime,
                    endTime = match.ScheduledDate.EndTime
                },
                userMatchedConfirmed = match.UserAConfirmed
            });
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
