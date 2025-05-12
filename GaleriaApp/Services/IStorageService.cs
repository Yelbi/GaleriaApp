using GaleriaApp.Models;

namespace GaleriaApp.Services
{
    public interface IStorageService
    {
        Task SaveMediaListAsync(List<MediaItem> mediaItems);
        Task<List<MediaItem>> LoadMediaListAsync();
        Task RemoveMediaItemAsync(string id);
    }
}