
using Mini_Dating_App_BE.DTOs.Requests;
using Mini_Dating_App_BE.DTOs.Responses;

namespace Mini_Dating_App_BE.Services.Interfaces
{
    public interface IUserService
    {
        Task<LoginRes> Login(LoginReq request);
        Task<bool> Register(RegisterReq request);
        Task<List<UserRes>> GetUserProfiles();

        Task Reset(int number);
    }
}
