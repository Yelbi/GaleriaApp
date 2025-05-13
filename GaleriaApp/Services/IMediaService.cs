using GaleriaApp.Models;

namespace GaleriaApp.Services
{
    public interface IMediaService
    {
        Task<List<MediaItem>> GetMediaFromDeviceAsync();
        Task<FileResult?> PickPhotoAsync();
        Task<FileResult?> PickVideoAsync();
        Task<FileResult?> TakePhotoAsync();
        Task<FileResult?> CaptureVideoAsync();
        Task<MediaItem?> GetMediaItemInfoAsync(string filePath);
    }
}