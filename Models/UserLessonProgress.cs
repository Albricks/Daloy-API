using daloy_api.Models;

public class UserLessonProgress
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid ModuleId { get; set; }
    public Guid LessonId { get; set; }

    public bool IsStarted { get; set; }
    public bool IsCompleted { get; set; }

    public int TimeSpentSeconds { get; set; }

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser User { get; set; } = default!;
    public Module Module { get; set; } = default!;
    public Lesson Lesson { get; set; } = default!;
}
