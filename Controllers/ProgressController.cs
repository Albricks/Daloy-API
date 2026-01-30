using daloy_api.DTOs;
using daloy_api.Models;
using daloy_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace daloy_api.Controllers
{
    [ApiController]
    [Route("api/progress")]
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpPost("lesson")]
        public async Task<IActionResult> UpdateLessonProgress(UpdateLessonProgressDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _progressService.UpdateLessonProgressAsync(userId, dto);
            return Ok();
        }

        [HttpPost("quiz")]
        public async Task<IActionResult> SubmitQuizAttempt(SubmitQuizAttemptDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _progressService.SubmitQuizAttemptAsync(userId, dto);
            return Ok();
        }
    }

}