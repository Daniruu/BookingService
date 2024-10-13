using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookingService.Services
{
    public class GoogleCloudStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "bookiteasy-data-bucket";

        public GoogleCloudStorageService()
        {
            string credentialPath = @"C:\Users\danil\source\repos\BookingService\BookingService\bookiteasy-437823-1218136af105.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            _storageClient = StorageClient.Create();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string objectName)
        {
            var obj = await _storageClient.UploadObjectAsync(_bucketName, objectName, null, fileStream);
            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }

        public async Task DeleteFileAsync(string objectName)
        {
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
                Console.WriteLine($"File {objectName} deleted successfully.");
            }
            catch (Google.GoogleApiException e)
            {
                if (e.Error.Code == 404)
                {
                    Console.WriteLine($"File {objectName} not found.");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
