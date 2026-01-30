using daloy_api.DTOs;

namespace daloy_api.Services.Interfaces
{
    public interface IProgressService
    {
        Task UpdateLessonProgressAsync(Guid userId, UpdateLessonProgressDto dto);
        Task SubmitQuizAttemptAsync(Guid userId, SubmitQuizAttemptDto dto);
        Task RecalculateModuleProgressAsync(Guid userId, Guid moduleId);
    }

}
