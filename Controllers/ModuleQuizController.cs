using daloy_api.Content.Quiz;
using daloy_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ModuleQuizController : ControllerBase
{
    private readonly AppDbContext _db;

    public ModuleQuizController(AppDbContext db)
    {
        _db = db;
    }

    // ============================
    // GET quiz by module
    // ============================
    // GET: /api/modulequiz/module/{moduleId}
    [HttpGet("module/{moduleId:guid}")]
    public async Task<IActionResult> GetQuizByModule(Guid moduleId)
    {
        var quiz = await _db.ModuleQuizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Choices)
            .FirstOrDefaultAsync(q => q.ModuleId == moduleId);

        if (quiz == null)
            return NotFound("Quiz not found for this module.");

        var dto = new ModuleQuizDto
        {
            QuizId = quiz.Id,
            ModuleId = quiz.ModuleId,
            Title = quiz.Title,
            Questions = quiz.Questions
                .OrderBy(q => q.Order)
                .Select(q => new QuizQuestionDto
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    Order = q.Order,
                    Choices = q.Choices
                        .Select(c => new QuizChoiceDto
                        {
                            ChoiceId = c.Id,
                            Label = c.ChoiceLabel,
                            Text = c.ChoiceText
                        })
                        .ToList()
                })
                .ToList()
        };

        return Ok(dto);
    }

    // ============================
    // POST submit quiz
    // ============================
    // POST: /api/modulequiz/submit
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizDto dto)
    {
        var quiz = await _db.ModuleQuizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Choices)
            .FirstOrDefaultAsync(q => q.Id == dto.QuizId);

        if (quiz == null)
            return NotFound("Quiz not found.");

        int totalQuestions = quiz.Questions.Count;
        int correctAnswers = 0;

        foreach (var answer in dto.Answers)
        {
            var question = quiz.Questions
                .FirstOrDefault(q => q.Id == answer.QuestionId);

            if (question == null)
                continue;

            var selectedChoice = question.Choices
                .FirstOrDefault(c => c.Id == answer.SelectedChoiceId);

            if (selectedChoice != null && selectedChoice.IsCorrect)
            {
                correctAnswers++;
            }
        }

        int scorePercentage = totalQuestions == 0
            ? 0
            : (int)Math.Round((double)correctAnswers / totalQuestions * 100);

        var result = new QuizResultDto
        {
            TotalQuestions = totalQuestions,
            CorrectAnswers = correctAnswers,
            ScorePercentage = scorePercentage
        };

        return Ok(result);
    }
}