using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CombatLink.Domain.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CombatLink.Infrastructure.BlobStorage
{
    public class BlobService : IBlobService
    {
        private string _connectionString;
        private string _containerName;

        public BlobService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _containerName = configuration["AzureBlobStorage:ContainerName"];
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File does not exist or is empty.");
            }

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = Guid.NewGuid().ToString() + extension;

            var blobClient = new BlobContainerClient(_connectionString, _containerName);
            await blobClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blob = blobClient.GetBlobClient(uniqueFileName);
            using var stream = file.OpenReadStream();
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            return blob.Uri.ToString();
        }

    }
}
