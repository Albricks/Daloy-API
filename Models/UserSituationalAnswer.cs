using System;

namespace daloy_api.Models
{
    public class UserSituationalAnswer
    {
        public Guid Id { get; set; }

        public Guid AttemptId { get; set; }

        public Guid QuestionId { get; set; }

        public string AnswerText { get; set; } = null!;

        public int WordCount { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public UserSituationalAttempt Attempt { get; set; } = null!;
        public SituationalQuestion Question { get; set; } = null!;
    }
}
