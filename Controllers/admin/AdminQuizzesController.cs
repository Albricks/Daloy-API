using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using daloy_api.DTOs.admin;

namespace daloy_api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/quizzes")]
    [Authorize(Roles = "Admin")]
    public class AdminQuizzesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminQuizzesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("module/{moduleId}")]
        public async Task<IActionResult> GetResultsByModule(Guid moduleId)
        {
            var result = await _context
                .Set<AdminQuizSummaryDto>()
                .FromSqlRaw("EXEC sp_Admin_GetQuizResultsByModule @ModuleId = {0}", moduleId)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAttemptsByUser(Guid userId)
        {
            var result = await _context
                .Set<AdminQuizSummaryDto>()
                .FromSqlRaw("EXEC sp_Admin_GetQuizAttemptsByUser @UserId = {0}", userId)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{quizId}/questions")]
        public async Task<IActionResult> GetQuizQuestions(Guid quizId)
        {
            var result = await _context
                .Set<AdminQuizQuestionDto>()
                .FromSqlRaw("EXEC sp_Admin_GetQuizQuestions @QuizId = {0}", quizId)
                .ToListAsync();

            return Ok(result);
        }
    }
}
