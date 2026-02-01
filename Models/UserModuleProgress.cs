using daloy_api.Models;

public class UserModuleProgress
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid ModuleId { get; set; }

    public ModuleStatus Status { get; set; }
    public int ProgressPercent { get; set; }

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser User { get; set; } = default!;
    public Module Module { get; set; } = default!;
}

public enum ModuleStatus
{
    New = 0,
    InProgress = 1,
    Completed = 2
}
