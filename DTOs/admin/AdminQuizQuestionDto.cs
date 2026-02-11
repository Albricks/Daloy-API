namespace daloy_api.DTOs.admin
{
    public class AdminQuizQuestionDto
    {
        public Guid QuizId { get; set; }
        public string QuizTitle { get; set; } = "";

        public Guid QuestionId { get; set; }
        public int QuestionOrder { get; set; }
        public string QuestionText { get; set; } = "";

        public string ChoiceLabel { get; set; } = "";
        public string ChoiceText { get; set; } = "";
        public bool IsCorrect { get; set; }
    }

}
