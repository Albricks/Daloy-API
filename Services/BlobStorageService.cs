using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace daloy_api.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration config)
        {
            var blobServiceClient = new BlobServiceClient(
                config["AzureBlob:ConnectionString"]
            );

            _container = blobServiceClient.GetBlobContainerClient(
                config["AzureBlob:ContainerName"]
            );
        }

        /// <summary>
        /// Uploads or replaces a user's avatar
        /// </summary>
        public async Task<string> UploadAvatarAsync(
            Guid userId,
            IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var blobName = $"{userId}{extension}";

            var blobClient = _container.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobName;
        }

        /// <summary>
        /// Generates a temporary read-only URL
        /// </summary>
        public string GetAvatarSasUrl(string blobName)
        {
            var blobClient = _container.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _container.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(12)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

    }
}
