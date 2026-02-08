using System;
using System.Collections.Generic;

namespace daloy_api.Models
{
    public class SituationalActivity
    {
        public Guid Id { get; set; }

        public Guid ModuleId { get; set; }

        public int SortOrder { get; set; }

        public string Title { get; set; } = null!;

        public string InstructionText { get; set; } = null!;

        public string ScenarioText { get; set; } = null!;

        public int MinWordCount { get; set; } = 25;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<SituationalQuestion> Questions { get; set; }
            = new List<SituationalQuestion>();
    }
}

