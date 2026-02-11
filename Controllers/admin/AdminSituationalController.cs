using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using daloy_api.DTOs.admin;

namespace daloy_api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/situational")]
    [Authorize(Roles = "Admin")]
    public class AdminSituationalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminSituationalController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("module/{moduleId}")]
        public async Task<IActionResult> GetByModule(Guid moduleId)
        {
            var result = await _context
                .Set<AdminSituationalSummaryDto>()
                .FromSqlRaw("EXEC sp_Admin_GetSituationalByModule @ModuleId = {0}", moduleId)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("answers")]
        public async Task<IActionResult> GetAnswers(
            [FromQuery] Guid userId,
            [FromQuery] Guid activityId)
        {
            var result = await _context
                .Set<AdminSituationalAnswerDto>()
                .FromSqlRaw(
                    "EXEC sp_Admin_GetSituationalAnswersByUser @UserId = {0}, @ActivityId = {1}",
                    userId, activityId)
                .ToListAsync();

            return Ok(result);
        }
    }
}
