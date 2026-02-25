using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.DTOs.Requests;
using Mini_Dating_App_BE.DTOs.Responses;
using Mini_Dating_App_BE.Hubs;
using Mini_Dating_App_BE.Repositories.Interfaces;
using Mini_Dating_App_BE.Services.Interfaces;

namespace Mini_Dating_App_BE.Services.Implements
{
    public class AvailabilityService : BaseService<AvailabilityService>, IAvailabilityService
    {
        public AvailabilityService(IHubContext<SystemHub> hubContext, IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<AvailabilityService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(hubContext, unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<List<SetUserAvailabilityRes>> GetAvailabilities()
        {
            var availabilities = await QueryAvailability();
            return availabilities.Select(a => _mapper.Map<SetUserAvailabilityRes>(a)).OrderBy(x => x.Date.Date).ToList();
        }

        private async Task<ICollection<Availability>> QueryAvailability()
        {
            return await _unitOfWork.GetRepository<Availability>().GetListAsync(predicate: x => x.UserId == GetUserId());
        }

        public async Task<bool> SettingAvailability(List<SetUserAvailabilityReq> requests)
        {
            ValidateAvailability(requests);

            var oldAvailabilities = await QueryAvailability();
            if (oldAvailabilities.Any()) _unitOfWork.GetRepository<Availability>().DeleteRange(oldAvailabilities);

            requests = requests.OrderBy(r => r.Date.Date).ToList();

            var userId = GetUserId();
            var newAvailabilities = requests.Select(r =>
            {
                var availability = _mapper.Map<Availability>(r);
                availability.UserId = userId;
                return availability;
            }).ToList();

            await _unitOfWork.GetRepository<Availability>().InsertRangeAsync(newAvailabilities);

            await _unitOfWork.CommitAsync();

            return true;
        }

        private void ValidateAvailability(List<SetUserAvailabilityReq> requests)
        {
            if (requests == null || !requests.Any() || requests.Count > 21 ||
                requests.Any(x => x == null || x.Date == default || x.StartTime == default || x.EndTime == default))
                throw new BadHttpRequestException("Invalid or empty availability list");

            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            if (requests.Any(x => x.Date.Date < tomorrow)) throw new BadHttpRequestException("Availability must be at least 1 day in the future.");

            if (requests.Any(x => x.Date.Date > DateTime.UtcNow.Date.AddDays(21))) throw new BadHttpRequestException("Availability cannot exceed 3 weeks from today.");

            if (requests.GroupBy(r => r.Date.Date).Any(g => g.Count() > 1)) throw new BadHttpRequestException("Duplicate availability dates are not allowed.");

            var maxTime = TimeSpan.FromDays(1);

            if (requests.Any(r =>
                r.StartTime < TimeSpan.Zero || r.StartTime >= maxTime ||
                r.EndTime <= TimeSpan.Zero || r.EndTime > maxTime ||
                r.StartTime >= r.EndTime))
            {
                throw new BadHttpRequestException("Invalid availability time slot. Times must be between 00:00 and 24:00, and Start must be before End.");
            }
        }
    }
}
