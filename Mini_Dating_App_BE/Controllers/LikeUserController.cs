using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini_Dating_App_BE.Services.Implements;
using Mini_Dating_App_BE.Services.Interfaces;

namespace Mini_Dating_App_BE.Controllers
{
    public class LikeUserController : BaseController<LikeUserController>
    {
        private readonly IUserLikeService _userLikeService;
        public LikeUserController(ILogger<LikeUserController> logger, IUserLikeService userLikeService) : base(logger)
        {
            _userLikeService = userLikeService;
        }

        [Authorize]
        [HttpPost("like/{likedId}")]
        public async Task<IActionResult> LikeUserProfile([FromRoute] Guid likedId)
        {
            await _userLikeService.LikeUserProfile(likedId);
            return Ok();
        }
    }
}
