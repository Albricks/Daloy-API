namespace daloy_api.Models
{
    public class UserVideoProgress
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Guid VideoId { get; set; }

        public int WatchedSeconds { get; set; }
        public int TotalSeconds { get; set; }

        // Stored as INT (0–100) in DB
        public int PercentWatched { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? LastWatchedAt { get; set; }

        // Navigation
        public AppUser User { get; set; } = default!;
        public Video Video { get; set; } = default!;
    }
}
