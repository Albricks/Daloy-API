namespace daloy_api.DTOs.admin
{
    public class AdminSituationalAnswerDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";

        public Guid ModuleId { get; set; }
        public string ModuleTitle { get; set; } = "";

        public Guid ActivityId { get; set; }
        public string ActivityTitle { get; set; } = "";

        public Guid QuestionId { get; set; }
        public int SortOrder { get; set; }
        public string QuestionText { get; set; } = "";

        public string AnswerText { get; set; } = "";
        public int WordCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
