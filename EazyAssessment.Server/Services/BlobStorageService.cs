using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace EazyAssessment.Server.Services
{
    public class BlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["BlobStorage:BlobConnection"]!;
            _containerName = configuration["BlobStorage:ContainerName"]!;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var containerClient = new BlobContainerClient(_connectionString, _containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";

            var blobClient = containerClient.GetBlobClient(fileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            await blobClient.UploadAsync(
                file.OpenReadStream(),
                new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

            return blobClient.Uri.ToString();
        }

        public async Task DeleteAsync(string blobUrl)
        {
            if (string.IsNullOrWhiteSpace(blobUrl))
                return;

            var containerClient = new BlobContainerClient(
                _connectionString,
                _containerName);

            var blobName = Path.GetFileName(new Uri(blobUrl).AbsolutePath);

            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}