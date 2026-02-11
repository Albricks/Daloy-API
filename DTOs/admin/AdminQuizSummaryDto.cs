namespace daloy_api.DTOs.admin
{
    public class AdminQuizSummaryDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";

        public Guid ModuleId { get; set; }
        public string ModuleTitle { get; set; } = "";

        public Guid QuizId { get; set; }
        public string QuizTitle { get; set; } = "";

        public int AttemptNumber { get; set; }
        public int Score { get; set; }
        public int TotalItems { get; set; }
        public decimal Percentage { get; set; }
        public bool IsPassed { get; set; }
        public DateTime AttemptedAt { get; set; }
    }

}
