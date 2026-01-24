using daloy_api.Models;

namespace daloy_api.Content.Videos
{
    public static class VideoMappings
    {
        public static VideoDto ToDto(this Video video)
        {
            return new VideoDto
            {
                Id = video.Id,
                Title = video.Title,


                Duration = video.Duration.HasValue
            ? video.Duration.Value.ToString(@"mm\:ss")
            : "00:00",


                ThumbnailUrl = video.BlobName, // TEMP (replace with real URL later)


                ModuleId = video.LearningModuleId,
                Order = video.Order,


                Status = video.Status switch
                {
                    VideoStatus.NotStarted => "not-started",
                    VideoStatus.InProgress => "in-progress",
                    VideoStatus.Completed => "completed",
                    _ => "not-started"
                }
            };
        }
    }
}
