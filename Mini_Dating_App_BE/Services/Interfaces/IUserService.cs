
using Mini_Dating_App_BE.DTOs.Requests;
using Mini_Dating_App_BE.DTOs.Responses;

namespace Mini_Dating_App_BE.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationRes> Login(LoginReq request);
        Task<AuthenticationRes> Register(RegisterReq request);
        Task<List<UserRes>> GetUserProfiles();
    }
}
