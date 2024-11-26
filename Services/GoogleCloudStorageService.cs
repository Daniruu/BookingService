using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookingService.Services
{
    public class GoogleCloudStorageService : IGoogleCloudStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly IConfiguration _configuration;
        private readonly string? _bucketName;

        public GoogleCloudStorageService(IConfiguration configuration)
        {
            var credentialPath = configuration["GoogleCloud:CredentialPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            _storageClient = StorageClient.Create();
            _bucketName = configuration["GoogleCloud:BucketName"];
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string objectName)
        {
            try
            {
                var obj = await _storageClient.UploadObjectAsync(_bucketName, objectName, null, fileStream);
                return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
            }
            catch (Google.GoogleApiException ex)
            {
                Console.WriteLine($"Error while uploading a file: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteFileAsync(string objectName)
        {
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
                Console.WriteLine($"File {objectName} deleted successfully.");
            }
            catch (Google.GoogleApiException ex)
            {
                if (ex.Error.Code == 404)
                {
                    Console.WriteLine($"File {objectName} not found.");
                    return;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
