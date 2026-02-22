using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini_Dating_App_BE.Services.Interfaces;

namespace Mini_Dating_App_BE.Controllers
{
    public class MatchController : BaseController<MatchController>
    {
        private readonly IMatchService _matchService;
        public MatchController(ILogger<MatchController> logger, IMatchService matchService) : base(logger)
        {
            _matchService = matchService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserMatches()
        {
            var response = await _matchService.GetUserMatches();
            return Ok(response);
        }

        [Authorize]
        [HttpPost("schedule/{matchId}")]
        public async Task<IActionResult> ScheduleMatch([FromRoute] Guid matchId)
        {
            var response = await _matchService.ScheduleMatch(matchId);
            return Ok(response);
        }
    }
}
