namespace daloy_api.DTOs.admin
{
    public class AdminVideoProgressDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";

        public Guid LearningModuleId { get; set; }
        public string LearningModuleTitle { get; set; } = "";

        public Guid VideoId { get; set; }
        public string VideoTitle { get; set; } = "";
        public int VideoOrder { get; set; }

        public int WatchedSeconds { get; set; }
        public int TotalSeconds { get; set; }
        public int PercentWatched { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? LastWatchedAt { get; set; }
    }

}
