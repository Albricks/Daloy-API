using daloy_api.DTOs;
using daloy_api.Models;
using daloy_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace daloy_api.Services
{
    public class ProgressService : IProgressService
    {
        private readonly AppDbContext _db;

        public ProgressService(AppDbContext db)
        {
            _db = db;
        }

        public async Task UpdateLessonProgressAsync(Guid userId, UpdateLessonProgressDto dto)
        {
            var progress = await _db.UserLessonProgresses
                .FirstOrDefaultAsync(x => x.UserId == userId && x.LessonId == dto.LessonId);

            if (progress == null)
            {
                progress = new UserLessonProgress
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ModuleId = dto.ModuleId,
                    LessonId = dto.LessonId,
                    IsStarted = true,
                    StartedAt = DateTime.UtcNow
                };

                _db.UserLessonProgresses.Add(progress);
            }

            progress.IsCompleted = dto.IsCompleted;
            progress.TimeSpentSeconds += dto.TimeSpentSeconds;
            progress.LastAccessedAt = DateTime.UtcNow;

            if (dto.IsCompleted)
            {
                progress.CompletedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            await RecalculateModuleProgressAsync(userId, dto.ModuleId);
        }

        public async Task SubmitQuizAttemptAsync(Guid userId, SubmitQuizAttemptDto dto)
        {
            var attemptCount = await _db.UserQuizAttempts
                .CountAsync(x => x.UserId == userId && x.QuizId == dto.QuizId);

            var attempt = new UserQuizAttempt
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ModuleId = dto.ModuleId,
                QuizId = dto.QuizId,
                Score = dto.Score,
                TotalItems = dto.TotalItems,
                Percentage = Math.Round((decimal)dto.Score / dto.TotalItems * 100, 2),
                AttemptNumber = attemptCount + 1,
                IsPassed = dto.Score >= (int)(dto.TotalItems * 0.7)
            };

            _db.UserQuizAttempts.Add(attempt);
            await _db.SaveChangesAsync();

            // Optional: auto-complete lesson if quiz passed
            if (attempt.IsPassed)
            {
                await RecalculateModuleProgressAsync(userId, dto.ModuleId);
            }
        }

        public async Task RecalculateModuleProgressAsync(Guid userId, Guid moduleId)
        {
            var totalVideos = await _db.Videos
                .CountAsync(x => x.LearningModuleId == moduleId);

            var completedVideos = await _db.UserVideoProgresses
                .CountAsync(x =>
                    x.UserId == userId &&
                    x.LearningModuleId == moduleId &&
                    x.IsCompleted);

            var moduleProgress = await _db.UserModuleProgresses
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ModuleId == moduleId);

            if (moduleProgress == null)
            {
                moduleProgress = new UserModuleProgress
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ModuleId = moduleId,
                    IsStarted = true,
                    StartedAt = DateTime.UtcNow
                };

                _db.UserModuleProgresses.Add(moduleProgress);
            }

            moduleProgress.TotalLessons = totalVideos;       // consider renaming later
            moduleProgress.CompletedLessons = completedVideos;
            moduleProgress.ProgressPercent =
                totalVideos == 0 ? 0 :
                Math.Round((decimal)completedVideos / totalVideos * 100, 2);

            moduleProgress.IsCompleted = completedVideos == totalVideos;
            moduleProgress.LastAccessedAt = DateTime.UtcNow;

            if (moduleProgress.IsCompleted)
            {
                moduleProgress.CompletedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

    }

}
