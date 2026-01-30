namespace daloy_api.DTOs
{
    public class SubmitQuizAttemptDto
    {
        public Guid ModuleId { get; set; }
        public Guid QuizId { get; set; }

        public int Score { get; set; }
        public int TotalItems { get; set; }
    }

}
