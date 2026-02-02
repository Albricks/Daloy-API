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

        // ============================
        // LESSON PROGRESS (SOURCE)
        // ============================
        public async Task UpdateLessonProgressAsync(
            Guid userId,
            UpdateLessonProgressDto dto)
        {
            var progress = await _db.UserLessonProgresses
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.LessonId == dto.LessonId);

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

            // 🔒 SAFETY GUARDS
            // --------------------

            // Ignore zero-time updates unless completing
            if (dto.TimeSpentSeconds <= 0 && !dto.IsCompleted)
                return;

            // Prevent double completion
            if (dto.IsCompleted && progress.IsCompleted)
                return;

            // Add time safely
            if (dto.TimeSpentSeconds > 0)
            {
                progress.TimeSpentSeconds += dto.TimeSpentSeconds;
            }

            progress.LastAccessedAt = DateTime.UtcNow;

            if (dto.IsCompleted)
            {
                progress.IsCompleted = true;
                progress.CompletedAt ??= DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            // 🔑 Recalculate module progress AFTER lesson update
            await RecalculateModuleProgressAsync(userId, dto.ModuleId);
        }


        // ============================
        // QUIZ ATTEMPTS (MODULE-LEVEL)
        // ============================
        public async Task SubmitQuizAttemptAsync(Guid userId, SubmitQuizAttemptDto dto)
        {
            var attemptCount = await _db.UserQuizAttempts
                .CountAsync(x =>
                    x.UserId == userId &&
                    x.QuizId == dto.QuizId);

            var percentage = dto.TotalItems == 0
                ? 0
                : Math.Round((decimal)dto.Score / dto.TotalItems * 100, 2);

            var attempt = new UserQuizAttempt
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ModuleId = dto.ModuleId,
                QuizId = dto.QuizId,
                Score = dto.Score,
                TotalItems = dto.TotalItems,
                Percentage = percentage,
                AttemptNumber = attemptCount + 1,
                IsPassed = percentage >= 70
            };

            _db.UserQuizAttempts.Add(attempt);
            await _db.SaveChangesAsync();

            // Quiz affects MODULE only
            await RecalculateModuleProgressAsync(userId, dto.ModuleId);
        }

        // ============================
        // MODULE PROGRESS (DERIVED)
        // ============================
        public async Task RecalculateModuleProgressAsync(Guid userId, Guid moduleId)
        {
            var totalLessons = await _db.Lessons
                .CountAsync(x => x.ModuleId == moduleId);

            var completedLessons = await _db.UserLessonProgresses
                .CountAsync(x =>
                    x.UserId == userId &&
                    x.ModuleId == moduleId &&
                    x.IsCompleted);

            var moduleProgress = await _db.UserModuleProgresses
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ModuleId == moduleId);

            if (moduleProgress == null)
            {
                moduleProgress = new UserModuleProgress
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ModuleId = moduleId,
                    Status = ModuleStatus.New,
                    ProgressPercent = 0,
                    StartedAt = DateTime.UtcNow,
                    LastAccessedAt = DateTime.UtcNow
                };

                _db.UserModuleProgresses.Add(moduleProgress);
            }

            var progressPercent = totalLessons == 0
                ? 0
                : (int)Math.Round(
                    (decimal)completedLessons / totalLessons * 100, 0);

            moduleProgress.ProgressPercent = progressPercent;
            moduleProgress.LastAccessedAt = DateTime.UtcNow;

            if (progressPercent == 0)
            {
                moduleProgress.Status = ModuleStatus.New;
                moduleProgress.CompletedAt = null;
            }
            else if (progressPercent == 100)
            {
                moduleProgress.Status = ModuleStatus.Completed;
                moduleProgress.CompletedAt ??= DateTime.UtcNow;
            }
            else
            {
                moduleProgress.Status = ModuleStatus.InProgress;
                moduleProgress.CompletedAt = null;
            }

            await _db.SaveChangesAsync();
        }
    }
}
