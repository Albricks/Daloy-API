namespace daloy_api.DTOs.admin
{
    public class AdminVideoCompletionDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";

        public string LearningModuleTitle { get; set; } = "";
        public string VideoTitle { get; set; } = "";

        public DateTime? LastWatchedAt { get; set; }
    }

}
