namespace BookingService.Services
{
    public interface IGoogleCloudStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string objectName);
        Task DeleteFileAsync(string objectName);

    }
}
