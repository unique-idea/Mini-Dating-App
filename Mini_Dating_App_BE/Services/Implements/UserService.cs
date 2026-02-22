using AutoMapper;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.DTOs.Requests;
using Mini_Dating_App_BE.DTOs.Responses;
using Mini_Dating_App_BE.Enums;
using Mini_Dating_App_BE.Repositories.Interfaces;
using Mini_Dating_App_BE.Services.Interfaces;


namespace Mini_Dating_App_BE.Services.Implements
{
    public class UserService : BaseService<UserService>, IUserService
    {
        private readonly IJwtService _jwtService;
        public UserService(IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IJwtService jwtService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _jwtService = jwtService;
        }

        public async Task<AuthenticationRes> Login(LoginReq request)
        {
            var user = await GetUserByEmailAsync(request.Email);

            if (user == null) throw new KeyNotFoundException("Email does not exist");

            return BuildAuthResponse(user);
        }

        public async Task<AuthenticationRes> Register(RegisterReq request)
        {
            if (!Enum.IsDefined(typeof(GenderEnum), request.Gender!)) throw new BadHttpRequestException("Invalid gender value.");

            var existingUser = await GetUserByEmailAsync(request.Email);

            if (existingUser != null) throw new BadHttpRequestException("Email already exists");

            var newUser = _mapper.Map<User>(request);

            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
            await _unitOfWork.CommitAsync();

            return BuildAuthResponse(newUser);
        }

        private async Task<User?> GetUserByEmailAsync(string email)
        {
            var normalizedEmail = email.ToLower().Trim();

            return await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Email.ToLower() == normalizedEmail);
        }

        private AuthenticationRes BuildAuthResponse(User user)
        {
            return new AuthenticationRes
            {
                User = _mapper.Map<UserRes>(user),
                Token = _jwtService.GenerateToken(user)
            };
        }

        public async Task<List<UserRes>> GetUserProfiles()
        {
            var users = await _unitOfWork.GetRepository<User>().GetListAsync(predicate: x => x.UserId != GetUserId());

            return users.Select(u => _mapper.Map<UserRes>(u)).ToList();
        }
    }
}

