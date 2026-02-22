using AutoMapper;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.DTOs.Requests;
using Mini_Dating_App_BE.DTOs.Responses;
using Mini_Dating_App_BE.Repositories.Interfaces;
using Mini_Dating_App_BE.Services.Interfaces;

namespace Mini_Dating_App_BE.Services.Implements
{
    public class AvailabilityService : BaseService<AvailabilityService>, IAvailabilityService
    {
        public AvailabilityService(IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<AvailabilityService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
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
            if (requests == null || !requests.Any() || requests.Count() > 21) throw new BadHttpRequestException("Invalid availability list");
            if (requests.Min(x => x.Date).Date < DateTime.UtcNow.Date) throw new BadHttpRequestException("Availability date cannot be in the past");
            if (requests.Max(x => x.Date).Date > DateTime.UtcNow.Date.AddDays(21)) throw new BadHttpRequestException("Availability date cannot be exceed more than 3 weeks");
            if (requests.GroupBy(r => r.Date.Date).Any(g => g.Count() > 1)) throw new BadHttpRequestException("Duplicate availability date is not allowed");

            if (requests.Any(r => r.StartTime < TimeSpan.Zero || r.StartTime > TimeSpan.FromHours(24)
               || r.EndTime < TimeSpan.Zero || r.EndTime > TimeSpan.FromHours(24) || r.StartTime >= r.EndTime))
                throw new BadHttpRequestException("Invalid availability time");
        }
    }
}
