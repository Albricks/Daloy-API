namespace daloy_api.Content.Quiz
{
    public class ModuleQuizDto
    {
        public Guid QuizId { get; set; }
        public Guid ModuleId { get; set; }
        public string Title { get; set; } = null!;
        public List<QuizQuestionDto> Questions { get; set; } = new();
    }
}
