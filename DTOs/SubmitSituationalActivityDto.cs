using System;
using System.Collections.Generic;

namespace daloy_api.DTOs
{
    public class SubmitSituationalActivityDto
    {
        public Guid ActivityId { get; set; }
        public List<SituationalAnswerDto> Answers { get; set; } = new();
    }

    public class SituationalAnswerDto
    {
        public Guid QuestionId { get; set; }
        public string AnswerText { get; set; } = null!;
    }
}
