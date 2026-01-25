namespace daloy_api.Models
{
    public class Lesson
    {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }

        public string Title { get; set; } = null!;

        public string ContentBlobPath { get; set; } = null!;

        public int Order { get; set; }

        public int? EstimatedMinutes { get; set; }
        public string LessonType { get; set; } = "Content"; 

        public Module Module { get; set; } = null!;
    }
}

