namespace daloy_api.Services.Interfaces
{
    public interface IAvatarService
    {
        // Validation
        void ValidateAvatar(IFormFile file);

        // Storage lifecycle
        Task DeleteAvatarIfExistsAsync(string? oldBlobName);

        Task<string> UploadAvatarAsync(IFormFile file, string userId);

        // Access
        string GetAvatarSasUrl(string blobName, int minutes = 60);
    }
}
