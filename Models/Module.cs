using daloy_api.Models;

namespace daloy_api.Models
{
    public class Module
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Level { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public int Lessons { get; set; }

        public ModuleStatus Status { get; set; }
        public int Order { get; set; }
        public ICollection<ModuleObjective> Objectives { get; set; } = new List<ModuleObjective>();
        public ICollection<Lesson> LessonsList { get; set; } = new List<Lesson>();
        public ModulePreviewStandard? PreviewStandard { get; set; }
    }

    public enum ModuleStatus
    {
        New = 0,
        InProgress = 1,
        Completed = 2
    }
}
