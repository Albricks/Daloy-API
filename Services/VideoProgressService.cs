using daloy_api.DTOs;
using daloy_api.Models;
using daloy_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace daloy_api.Services
{
    public class VideoProgressService : IVideoProgressService
    {
        private readonly AppDbContext _db;

        private const int COMPLETION_THRESHOLD = 80; // percent

        public VideoProgressService(AppDbContext db)
        {
            _db = db;
        }

        public async Task UpdateVideoProgressAsync(Guid userId, UpdateVideoProgressDto dto)
        {
            var video = await _db.Videos
                .SingleAsync(v => v.Id == dto.VideoId);

            var totalSeconds = (int)(video.Duration?.TotalSeconds ?? 0);

            if (totalSeconds <= 0)
                return; // safety guard

            var progress = await _db.UserVideoProgresses
                .SingleOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.VideoId == dto.VideoId);

            if (progress == null)
            {
                progress = new UserVideoProgress
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    VideoId = dto.VideoId,
                    WatchedSeconds = 0,
                    TotalSeconds = totalSeconds,
                    PercentWatched = 0,
                    IsCompleted = false
                };

                _db.UserVideoProgresses.Add(progress);
            }

            // Anti-skip protection
            progress.WatchedSeconds = Math.Max(
                progress.WatchedSeconds,
                dto.WatchedSeconds);

            progress.TotalSeconds = totalSeconds;

            progress.PercentWatched = (int)Math.Min(
                100,
                Math.Round(
                    (decimal)progress.WatchedSeconds / totalSeconds * 100, 0));

            progress.IsCompleted =
                progress.PercentWatched >= COMPLETION_THRESHOLD;

            progress.LastWatchedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
    }
}
