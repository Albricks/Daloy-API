using System;

namespace daloy_api.Models
{
    public class SituationalQuestion
    {
        public Guid Id { get; set; }

        public Guid ActivityId { get; set; }

        public string QuestionText { get; set; } = null!;

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
