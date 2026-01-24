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
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentException("Thumbnail blobName is required", nameof(blobName));


            var container = _blobServiceClient
            .GetBlobContainerClient(_videoContainer);


            var blob = container.GetBlobClient(blobName);


            return GenerateSas(blob, BlobSasPermissions.Read, TimeSpan.FromHours(1));
        }


        private string GenerateSas(
                                    BlobClient blob,
                                    BlobSasPermissions permissions,
                                    TimeSpan lifetime)
        {
            if (!blob.CanGenerateSasUri)
            {
                throw new InvalidOperationException(
                "BlobClient cannot generate SAS URI. Check credentials.");
            }


            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                Resource = "b", // blob
                ExpiresOn = DateTimeOffset.UtcNow.Add(lifetime)
            };


            sasBuilder.SetPermissions(permissions);


            var sasUri = blob.GenerateSasUri(sasBuilder);


            return sasUri.ToString();
        }
    }
}
