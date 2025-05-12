using GaleriaApp.Models;

namespace GaleriaApp.Services
{
    public interface IMediaService
    {
        Task<List<MediaItem>> GetMediaFromDeviceAsync();
        Task<FileResult?> PickPhotoAsync(); // Nota el signo ? para permitir null
        Task<FileResult?> PickVideoAsync(); // Nota el signo ? para permitir null
    }
}
