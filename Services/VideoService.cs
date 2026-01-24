using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using daloy_api.Services.Interfaces;
using System.Net;

namespace daloy_api.Services
{
    public class VideoService : IVideoService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _videoContainer;

        public VideoService(
            BlobServiceClient blobServiceClient,
            IConfiguration config)
        {
            _blobServiceClient = blobServiceClient;
            _videoContainer = config["AzureBlob:VideosContainer"]
                ?? throw new Exception("AzureBlob:VideosContainer not configured.");
        }

        public string GetVideoSasUrl(string blobName, int minutes = 120)
        {
            var container = _blobServiceClient
                .GetBlobContainerClient(_videoContainer);

            var blob = container.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _videoContainer,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(minutes)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blob.GenerateSasUri(sasBuilder).ToString();
        }

        public string GetThumbnailUrl(string blobName)
        {
            var safe = WebUtility.UrlEncode(blobName);
            return $"https://via.placeholder.com/320x180.png?text=Thumbnail+for+{safe}";

        }

    }
}
