namespace daloy_api.DTOs.admin
{
    public class AdminUserOverallProgressDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";

        public Guid ModuleId { get; set; }
        public string ModuleTitle { get; set; } = "";

        public int ModuleStatus { get; set; }
        public int ModuleProgressPercent { get; set; }
        public DateTime? ModuleCompletedAt { get; set; }

        public bool SituationalCompleted { get; set; }
        public DateTime? SituationalCompletedAt { get; set; }

        public bool QuizPassed { get; set; }
        public DateTime? LastQuizAttemptAt { get; set; }

        public decimal? AvgVideoPercentWatched { get; set; }
        public bool AllVideosCompleted { get; set; }
    }

}
