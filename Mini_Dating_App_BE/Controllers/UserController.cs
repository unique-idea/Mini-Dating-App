using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini_Dating_App_BE.DTOs.Requests;
using Mini_Dating_App_BE.Services.Interfaces;
using System.Security.Claims;

namespace Mini_Dating_App_BE.Controllers
{
    public class UserController : BaseController<UserController>
    {
        private readonly IUserService _userService;
        public UserController(ILogger<UserController> logger, IUserService userService) : base(logger)
        {
            _userService = userService;
        }
        [HttpPost("ResetAll")]
        public async Task<IActionResult> Reset([FromBody] int number)
        {

            await _userService.Reset(number);
            return Ok();

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginReq request)
        {

            var response = await _userService.Login(request);
            return Ok(response);

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterReq request)
        {
            var response = await _userService.Register(request);
            return Ok(response);

        }


        [Authorize]
        [HttpGet("profiles")]
        public async Task<IActionResult> GetUserProfiles()
        {
            var responses = await _userService.GetUserProfiles();
            return Ok(responses);
        }
    }
}
