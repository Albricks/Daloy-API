using daloy_api.DTOs;

namespace daloy_api.Services.Interfaces
{
    public interface IVideoProgressService
    {
        Task UpdateVideoProgressAsync(Guid userId, UpdateVideoProgressDto dto);
    }

}
