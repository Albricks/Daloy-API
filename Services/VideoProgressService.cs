using daloy_api.DTOs;
using daloy_api.Models;
using daloy_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace daloy_api.Services
{
    public class VideoProgressService : IVideoProgressService
    {
        private readonly AppDbContext _db;
        private readonly IProgressService _progressService;

        private const decimal COMPLETION_THRESHOLD = 0.80m; // 80%

        public VideoProgressService(
            AppDbContext db,
            IProgressService progressService)
        {
            _db = db;
            _progressService = progressService;
        }

        public async Task UpdateVideoProgressAsync(Guid userId, UpdateVideoProgressDto dto)
        {
            var video = await _db.Videos
                .SingleAsync(v => v.Id == dto.VideoId);

            var durationSeconds = (int)(video.Duration?.TotalSeconds ?? 0);

            var progress = await _db.UserVideoProgresses
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.VideoId == dto.VideoId);

            if (progress == null)
            {
                progress = new UserVideoProgress
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    VideoId = dto.VideoId,
                    LearningModuleId = video.LearningModuleId,
                    DurationSeconds = durationSeconds,
                    FirstPlayedAt = DateTime.UtcNow
                };

                _db.UserVideoProgresses.Add(progress);
            }

            // Anti-skip protection
            progress.WatchedSeconds = Math.Max(
                progress.WatchedSeconds,
                dto.WatchedSeconds);

            progress.DurationSeconds = durationSeconds;
            progress.LastPlayedAt = DateTime.UtcNow;

            progress.WatchedPercent =
                durationSeconds == 0
                    ? 0
                    : Math.Round(
                        (decimal)progress.WatchedSeconds / durationSeconds,
                        4);

            if (!progress.IsCompleted &&
                progress.WatchedPercent >= COMPLETION_THRESHOLD)
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;

                // Recalculate module progress based on videos
                await _progressService.RecalculateModuleProgressAsync(
                    userId,
                    video.LearningModuleId);
            }

            await _db.SaveChangesAsync();
        }
    }

}
