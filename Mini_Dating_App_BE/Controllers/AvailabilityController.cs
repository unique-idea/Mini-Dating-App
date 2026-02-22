using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini_Dating_App_BE.DTOs.Requests;
using Mini_Dating_App_BE.Services.Interfaces;

namespace Mini_Dating_App_BE.Controllers
{
    public class AvailabilityController : BaseController<AvailabilityController>
    {
        private readonly IAvailabilityService _availabilityService;
        public AvailabilityController(ILogger<AvailabilityController> logger, IAvailabilityService availabilityService) : base(logger)
        {
            _availabilityService = availabilityService;
        }

        [Authorize]
        [HttpPost("setting")]
        public async Task<IActionResult> SettingAvailability([FromBody] List<SetUserAvailabilityReq> requests)
        {
            var response = await _availabilityService.SettingAvailability(requests);
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAvailabilities()
        {
            var responses = await _availabilityService.GetAvailabilities();
            return Ok(responses);
        }
    }
}
