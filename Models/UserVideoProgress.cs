namespace daloy_api.Models
{
    public class UserVideoProgress
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        // Align with Videos.LearningModuleId
        public Guid LearningModuleId { get; set; }
        public Guid VideoId { get; set; }

        public int DurationSeconds { get; set; }
        public int WatchedSeconds { get; set; }
        public decimal WatchedPercent { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? FirstPlayedAt { get; set; }
        public DateTime? LastPlayedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public AppUser User { get; set; } = default!;
        public Video Video { get; set; } = default!;
        public Module Module { get; set; } = default!;
    }


}
