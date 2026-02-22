using Mini_Dating_App_BE.DTOs.Requests;
using Mini_Dating_App_BE.DTOs.Responses;

namespace Mini_Dating_App_BE.Services.Interfaces
{
    public interface IAvailabilityService
    {
        Task<bool> SettingAvailability(List<SetUserAvailabilityReq> requests);
        Task<List<SetUserAvailabilityRes>> GetAvailabilities();
    }
}
