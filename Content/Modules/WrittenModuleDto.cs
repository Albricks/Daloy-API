namespace daloy_api.Content.Modules
{
    public class WrittenModuleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int ReadTimeMinutes { get; set; }
        public string Status { get; set; } = default!;
        public string ThumbnailUrl { get; set; } = default!;
    }
}
