namespace daloy_api.Content.Quiz
{
    public class QuizQuestionDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = null!;
        public int Order { get; set; }
        public List<QuizChoiceDto> Choices { get; set; } = new();
    }
}
