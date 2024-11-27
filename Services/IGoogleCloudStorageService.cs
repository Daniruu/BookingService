namespace BookingService.Services
{
    public interface IGoogleCloudStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string folder, string fileName);
        Task DeleteFileAsync(string folder, string fileName);
    }
}
