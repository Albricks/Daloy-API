using daloy_api.Models;

public class UserQuizAttempt
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid ModuleId { get; set; }
    public Guid QuizId { get; set; }

    public int Score { get; set; }
    public int TotalItems { get; set; }

    public decimal Percentage { get; set; }
    public int AttemptNumber { get; set; }

    public bool IsPassed { get; set; }

    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser User { get; set; } = default!;
    public Module Module { get; set; } = default!;
    public ModuleQuiz Quiz { get; set; } = default!;
}
