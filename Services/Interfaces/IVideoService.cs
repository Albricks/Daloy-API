namespace daloy_api.Services.Interfaces
{
    public interface IVideoService
    {
        string GetVideoSasUrl(string blobName, int minutes = 120);
        string GetThumbnailUrl(string blobName);
    }
}
