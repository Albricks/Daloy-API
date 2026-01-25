namespace daloy_api.Content.Modules
{
    public class ModuleListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string Duration { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int Progress { get; set; }
    }
}
