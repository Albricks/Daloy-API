using daloy_api.Content.Videos;

namespace daloy_api.Models
{

    public class Video
    {
        public Guid Id { get; set; }


        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;


        public string BlobName { get; set; } = null!;


        public Guid LearningModuleId { get; set; }


        public int Order { get; set; }


        public TimeSpan? Duration { get; set; }


        // 👇 ADD THIS
        public VideoStatus Status { get; set; } = VideoStatus.NotStarted;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
