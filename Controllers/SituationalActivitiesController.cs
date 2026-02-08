using daloy_api.Data;
using daloy_api.DTOs;
using daloy_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace daloy_api.Controllers
{
    [ApiController]
    [Route("api/situational-activities")]
    [Authorize]
    public class SituationalActivitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SituationalActivitiesController(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // GET activity + questions
        // ============================
        [HttpGet("{activityId:guid}")]
        public async Task<IActionResult> GetActivity(Guid activityId)
        {
            var activity = await _context.SituationalActivities
                .Where(a => a.Id == activityId && a.IsActive)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.InstructionText,
                    a.ScenarioText,
                    a.MinWordCount,
                    Questions = _context.SituationalQuestions
                        .Where(q => q.ActivityId == a.Id)
                        .OrderBy(q => q.SortOrder)
                        .Select(q => new
                        {
                            q.Id,
                            q.QuestionText,
                            q.SortOrder
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (activity == null)
                return NotFound("Situational activity not found.");

            return Ok(activity);
        }

        // ============================
        // SUBMIT answers
        // ============================
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitSituationalActivityDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var activity = await _context.SituationalActivities
                .FirstOrDefaultAsync(a => a.Id == dto.ActivityId && a.IsActive);

            if (activity == null)
                return NotFound("Situational activity not found.");

            // 1️⃣ Get or create attempt
            var attempt = await _context.UserSituationalAttempts
                .FirstOrDefaultAsync(a =>
                    a.UserId == userId &&
                    a.ActivityId == dto.ActivityId);

            if (attempt == null)
            {
                attempt = new UserSituationalAttempt
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ActivityId = dto.ActivityId
                };

                _context.UserSituationalAttempts.Add(attempt);
            }

            var questions = await _context.SituationalQuestions
                .Where(q => q.ActivityId == dto.ActivityId)
                .ToListAsync();

            if (dto.Answers.Count != questions.Count)
                return BadRequest("All questions must be answered.");

            foreach (var answer in dto.Answers)
            {
                var questionExists = questions.Any(q => q.Id == answer.QuestionId);
                if (!questionExists)
                    return BadRequest("Invalid question detected.");

                var wordCount = answer.AnswerText
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Length;

                if (wordCount < activity.MinWordCount)
                    return BadRequest(
                        $"Each answer must be at least {activity.MinWordCount} words."
                    );

                // 2️⃣ UPDATE or INSERT answer
                var existingAnswer = await _context.UserSituationalAnswers
                    .FirstOrDefaultAsync(a =>
                        a.AttemptId == attempt.Id &&
                        a.QuestionId == answer.QuestionId);

                if (existingAnswer != null)
                {
                    existingAnswer.AnswerText = answer.AnswerText;
                    existingAnswer.WordCount = wordCount;
                }
                else
                {
                    _context.UserSituationalAnswers.Add(new UserSituationalAnswer
                    {
                        Id = Guid.NewGuid(),
                        AttemptId = attempt.Id,
                        QuestionId = answer.QuestionId,
                        AnswerText = answer.AnswerText,
                        WordCount = wordCount
                    });
                }
            }

            // 3️⃣ Always refresh completion timestamp
            attempt.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Situational activity saved successfully."
            });
        }

        [HttpGet("by-module/{moduleId}/completed")]
        [Authorize]
        public async Task<IActionResult> HasCompletedByModule(Guid moduleId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var activityId = await _context.SituationalActivities
                .Where(a => a.ModuleId == moduleId && a.IsActive)
                .OrderBy(a => a.SortOrder)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (activityId == Guid.Empty)
                return Ok(new { completed = false });

            var completed = await _context.UserSituationalAttempts
                .AnyAsync(a =>
                    a.ActivityId == activityId &&
                    a.UserId == userId &&
                    a.CompletedAt != null
                );

            return Ok(new { completed });
        }


        [HttpGet("by-module/{moduleId}")]
        [Authorize]
        public async Task<IActionResult> GetByModule(Guid moduleId)
        {
            var activity = await _context.SituationalActivities
                .Where(a => a.ModuleId == moduleId && a.IsActive)
                .OrderBy(a => a.SortOrder)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.InstructionText,
                    a.ScenarioText,
                    a.MinWordCount,
                    Questions = _context.SituationalQuestions
                        .Where(q => q.ActivityId == a.Id)
                        .OrderBy(q => q.SortOrder)
                        .Select(q => new
                        {
                            q.Id,
                            q.QuestionText,
                            q.SortOrder
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (activity == null)
                return NotFound("No situational activity found for this module.");

            return Ok(activity);
        }


    }
}
