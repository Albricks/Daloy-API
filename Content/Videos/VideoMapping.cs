using daloy_api.Models;
using daloy_api.Services;
using daloy_api.Services.Interfaces;

namespace daloy_api.Content.Videos
{
    public static class VideoMappings
    {
        public static VideoDto ToDto(this Video video, IVideoService videoService)
        {
            return new VideoDto
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                DurationSeconds = video.Duration.HasValue
                ? (int)video.Duration.Value.TotalSeconds
:                0,

                Duration = video.Duration.HasValue
            ? video.Duration.Value.ToString(@"mm\:ss")
            : "00:00",


                ThumbnailUrl = !string.IsNullOrWhiteSpace(video.ThumbnailBlobName)
                ? videoService.GetThumbnailUrl(video.ThumbnailBlobName)
                : videoService.GetThumbnailUrl("modules/shared/thumbnails/default.jpg"),


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
