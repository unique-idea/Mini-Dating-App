using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Hubs;
using Mini_Dating_App_BE.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Mini_Dating_App_BE.Services
{
    public abstract class BaseService<T> where T : class
    {
        protected readonly IHubContext<SystemHub> _hubContext;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IUnitOfWork<MiniDatingAppDbContext> _unitOfWork;
        protected ILogger<T> _logger;
        protected IMapper _mapper;

        //public BaseService(ILogger<T> logger, IMapper mapper, IHubContext<SystemHub> hubContext)
        public BaseService(IHubContext<SystemHub> hubContext ,IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<T> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
        }

        public Guid GetUserId()
        {
            return Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        }

        protected async Task NotifyUser(Guid userId, string eventName, object data)
        {
            await _hubContext.Clients.Group(userId.ToString())
                .SendAsync(eventName, data);
        }

        protected async Task NotifyUsers(IEnumerable<Guid> userIds, string eventName, object data)
        {
            foreach (var userId in userIds)
            {
                await _hubContext.Clients.Group(userId.ToString())
                    .SendAsync(eventName, data);
            }
        }
    }
}
