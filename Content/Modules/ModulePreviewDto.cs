namespace daloy_api.Content.Modules
{
    public class ModulePreviewDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string Duration { get; set; } = null!;
        public int Lessons { get; set; }
        public string Status { get; set; } = null!;
        public List<string> Objectives { get; set; } = new();
    }
}
