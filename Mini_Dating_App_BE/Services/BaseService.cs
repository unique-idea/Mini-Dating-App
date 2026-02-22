using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Mini_Dating_App_BE.Data;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.Repositories.Interfaces;
using Mini_Dating_App_BE.Services.Implements;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mini_Dating_App_BE.Services
{
    public abstract class BaseService<T> where T : class
    {
        //protected readonly IHubContext<SystemHub> _hubContext;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IUnitOfWork<MiniDatingAppDbContext> _unitOfWork;
        protected ILogger<T> _logger;
        protected IMapper _mapper;

        //public BaseService(ILogger<T> logger, IMapper mapper, IHubContext<SystemHub> hubContext)
        public BaseService(IUnitOfWork<MiniDatingAppDbContext> unitOfWork, ILogger<T> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            //_hubContext = hubContext;
        }

        public Guid GetUserId()
        {
            return Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        }

    }
}
