using daloy_api.DTOs;
using daloy_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace daloy_api.Controllers
{
    [ApiController]
    [Route("api/video-progress")]
    [Authorize]
    public class VideoProgressController : ControllerBase
    {
        private readonly IVideoProgressService _videoProgressService;

        public VideoProgressController(IVideoProgressService videoProgressService)
        {
            _videoProgressService = videoProgressService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVideoProgress(UpdateVideoProgressDto dto)
        {
            var userId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _videoProgressService.UpdateVideoProgressAsync(userId, dto);
            return Ok();
        }
    }

}
