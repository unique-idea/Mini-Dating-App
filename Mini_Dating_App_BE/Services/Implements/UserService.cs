using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.DTOs.Requests;
using Mini_Dating_App_BE.DTOs.Responses;
using Mini_Dating_App_BE.Enums;
using Mini_Dating_App_BE.Hubs;
using Mini_Dating_App_BE.Repositories.Interfaces;
using Mini_Dating_App_BE.Services.Interfaces;


namespace Mini_Dating_App_BE.Services.Implements
{
    public class UserService : BaseService<UserService>, IUserService
    {
        private readonly IJwtService _jwtService;
        public UserService(IHubContext<SystemHub> hubContext, IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IJwtService jwtService) : base(hubContext, unitOfWork, logger, mapper, httpContextAccessor)
        {
            _jwtService = jwtService;
        }

        public async Task<LoginRes> Login(LoginReq request)
        {
            var user = await GetUserByEmailAsync(request.Email);

            if (user == null) throw new KeyNotFoundException("Email does not exist");

            return BuildAuthResponse(user);
        }

        public async Task<bool> Register(RegisterReq request)
        {
            if (!Enum.IsDefined(typeof(GenderEnum), request.Gender!)) throw new BadHttpRequestException("Invalid gender value.");

            var existingUser = await GetUserByEmailAsync(request.Email);

            if (existingUser != null) throw new BadHttpRequestException("Email already exists");

            var newUser = _mapper.Map<User>(request);

            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
            await _unitOfWork.CommitAsync();

            return true;
        }

        private async Task<User?> GetUserByEmailAsync(string email)
        {
            var normalizedEmail = email.ToLower().Trim();

            return await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Email.ToLower() == normalizedEmail);
        }

        private LoginRes BuildAuthResponse(User user)
        {
            return new LoginRes
            {
                User = _mapper.Map<UserRes>(user),
                Token = _jwtService.GenerateToken(user)
            };
        }

        public async Task<List<UserRes>> GetUserProfiles()
        {
            var currentUserId = GetUserId();

            var users = await _unitOfWork.GetRepository<User>()
                .GetListAsync(predicate: x => x.UserId != currentUserId && !x.LikesReceived.Any(lr => lr.LikerId == currentUserId));

            return users.Select(u => _mapper.Map<UserRes>(u)).ToList();
        }

        public async Task Reset(int number)
        {
            var users = await _unitOfWork.GetRepository<User>().GetListAsync();
            var likes = await _unitOfWork.GetRepository<UserLike>().GetListAsync();
            var matches = await _unitOfWork.GetRepository<Match>().GetListAsync();
            var availabilities = await _unitOfWork.GetRepository<Availability>().GetListAsync();

            if (number == 1) _unitOfWork.GetRepository<Match>().DeleteRange(matches);
            if (number == 2) _unitOfWork.GetRepository<UserLike>().DeleteRange(likes);
            if (number == 3) _unitOfWork.GetRepository<Availability>().DeleteRange(availabilities);
            if (number == 4) _unitOfWork.GetRepository<User>().DeleteRange(users);






            await _unitOfWork.CommitAsync();

        }
    }
}

