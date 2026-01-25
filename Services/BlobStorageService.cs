using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace daloy_api.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _avatarContainer;
        private readonly BlobContainerClient _videosContainer;
        private readonly BlobContainerClient _modulesContainer;


        public BlobStorageService(IConfiguration config)
        {
            var connectionString = config["AzureBlob:ConnectionString"];
            var avatarContainerName = config["AzureBlob:AvatarContainer"];
            var videosContainerName = config["AzureBlob:VideosContainer"];
            var modulesContainerName = config["AzureBlob:ModulesContainer"];


            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("AzureBlob:ConnectionString is missing.");


            if (string.IsNullOrWhiteSpace(modulesContainerName))
                throw new InvalidOperationException("AzureBlob:ModulesContainer is missing.");


            var blobServiceClient = new BlobServiceClient(connectionString);


            _avatarContainer = blobServiceClient.GetBlobContainerClient(avatarContainerName);
            _videosContainer = blobServiceClient.GetBlobContainerClient(videosContainerName);
            _modulesContainer = blobServiceClient.GetBlobContainerClient(modulesContainerName);
        }

        // =======================
        // AVATAR METHODS (EXISTING)
        // =======================

        public async Task<string> UploadAvatarAsync(
            Guid userId,
            IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var blobName = $"{userId}{extension}";

            var blobClient = _avatarContainer.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobName;
        }

        public string GetAvatarSasUrl(string blobName)
        {
            var blobClient = _avatarContainer.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _avatarContainer.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(12)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }


        public string GetLessonSasUrl(string blobPath)
        {
            var blobClient = _modulesContainer.GetBlobClient(blobPath);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _modulesContainer.Name,
                BlobName = blobPath,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        public string GetModulesContainerSas()
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _modulesContainer.Name,
                Resource = "c", // 🔥 CONTAINER LEVEL
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(1),
                Protocol = SasProtocol.Https,
                Version = "2023-11-03"
            };

            sasBuilder.SetPermissions(
            BlobContainerSasPermissions.Read |
            BlobContainerSasPermissions.List
            );

            var sasUri = _modulesContainer.GenerateSasUri(sasBuilder);

            return sasUri.Query;
        }

    }
}