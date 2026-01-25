namespace daloy_api.Content.Quiz
{
    public class QuizChoiceDto
    {
        public Guid ChoiceId { get; set; }
        public string Label { get; set; } = null!;
        public string Text { get; set; } = null!;
    }
}
