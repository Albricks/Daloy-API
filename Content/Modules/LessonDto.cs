namespace daloy_api.Content.Modules
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;

        public string ContentUrl { get; set; } = null!;
        public string ContainerSas { get; set; } = null!;
        public int Order { get; set; }

        public int? EstimatedMinutes { get; set; }
        public string LessonType { get; set; } = "Content";
    }
}