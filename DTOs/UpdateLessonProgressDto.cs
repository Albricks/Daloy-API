namespace daloy_api.DTOs
{
    public class UpdateLessonProgressDto
    {
        public Guid ModuleId { get; set; }
        public Guid LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public int TimeSpentSeconds { get; set; }
    }

}
