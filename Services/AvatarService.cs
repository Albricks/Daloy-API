using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using daloy_api.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace daloy_api.Services
{
    public class AvatarService : IAvatarService
    {
        private const long MaxAvatarSizeBytes = 2 * 1024 * 1024; // 2 MB

        private static readonly Dictionary<string, HashSet<string>> AllowedExtensionsByType =
            new()
            {
                ["image/jpeg"] = new HashSet<string> { ".jpg", ".jpeg" },
                ["image/png"] = new HashSet<string> { ".png" },
                ["image/webp"] = new HashSet<string> { ".webp" }
            };

        private readonly BlobServiceClient _blobServiceClient;

        public AvatarService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        // ✅ Validation belongs here
        public void ValidateAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Avatar file is required.");

            if (file.Length > MaxAvatarSizeBytes)
                throw new ArgumentException("Avatar must be less than 2MB.");

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

            if (!AllowedExtensionsByType.TryGetValue(file.ContentType, out var allowedExtensions))
                throw new ArgumentException("Only JPG, PNG, and WEBP images are allowed.");

            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                throw new ArgumentException("File extension does not match image type.");
        }

        public async Task DeleteAvatarIfExistsAsync(string? oldBlobName)
        {
            if (string.IsNullOrWhiteSpace(oldBlobName))
                return;

            var container = _blobServiceClient.GetBlobContainerClient("avatars");
            var blob = container.GetBlobClient(oldBlobName);

            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> UploadAvatarAsync(
        IFormFile file,
        string userId)
        {
            var extension = Path.GetExtension(file.FileName);
            var blobName = GenerateAvatarBlobName(userId, extension);

            var container = _blobServiceClient.GetBlobContainerClient("avatars");
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();

            await blob.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

            return blobName;
        }

        public string GetAvatarSasUrl(string blobName, int minutes = 60)
        {
            var container = _blobServiceClient.GetBlobContainerClient("avatars");
            var blob = container.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(minutes)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blob.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }

        private string GenerateAvatarBlobName(string userId, string extension)
        {
            return $"{userId}/{Guid.NewGuid()}{extension}";
        }
    }

}
