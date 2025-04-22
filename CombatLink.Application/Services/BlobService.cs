using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CombatLink.Domain.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.Services
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

        public async Task<string> UploadImageAsync(IFormFile file, string fileName)
        {
            if(file == null || file.Length == 0)
            {
                throw new ArgumentException("There is no file uploaded or the uploaded one is empty.");
            }

            BlobContainerClient blobClient = new BlobContainerClient(_connectionString, _containerName);
            await blobClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            BlobClient blob = blobClient.GetBlobClient(fileName);
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }
            return blob.Uri.ToString();
        }
    }
}
