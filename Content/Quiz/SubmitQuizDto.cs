namespace daloy_api.Content.Quiz
{
    public class SubmitQuizDto
    {
        public Guid QuizId { get; set; }
        public List<SubmitQuizAnswerDto> Answers { get; set; } = new();
    }

    public class SubmitQuizAnswerDto
    {
        public Guid QuestionId { get; set; }
        public Guid SelectedChoiceId { get; set; }
    }
}
