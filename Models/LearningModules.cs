namespace daloy_api.Models
{
    public class LearningModule
    {
        public Guid Id { get; set; }


        // Subject / Theme
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;


        // Display ordering on homepage
        public int Order { get; set; }


        // Optional visuals
        public string? ThumbnailBlobName { get; set; }


        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Optional navigation (not required for MVP)
        // public ICollection<Video> Videos { get; set; }
    }
}
