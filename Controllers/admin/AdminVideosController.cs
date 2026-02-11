using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using daloy_api.DTOs.admin;

namespace daloy_api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/videos")]
    [Authorize(Roles = "Admin")]
    public class AdminVideosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminVideosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("module/{learningModuleId}")]
        public async Task<IActionResult> GetByLearningModule(Guid learningModuleId)
        {
            var result = await _context
                .Set<AdminVideoProgressDto>()
                .FromSqlRaw(
                    "EXEC sp_Admin_GetVideoProgressByModule @LearningModuleId = {0}",
                    learningModuleId)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var result = await _context
                .Set<AdminVideoProgressDto>()
                .FromSqlRaw(
                    "EXEC sp_Admin_GetVideoProgressByUser @UserId = {0}",
                    userId)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedVideos()
        {
            var result = await _context
                .Set<AdminVideoCompletionDto>()
                .FromSqlRaw("EXEC sp_Admin_GetVideoCompletion")
                .ToListAsync();

            return Ok(result);
        }
    }
}
