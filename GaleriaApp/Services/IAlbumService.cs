using GaleriaApp.Models; // Añadir este using

namespace GaleriaApp.Services
{
    public interface IAlbumService
    {
        Task<List<Album>> GetAlbumsAsync();
        Task<Album> GetAlbumByIdAsync(string id);
        Task SaveAlbumAsync(Album album);
        Task DeleteAlbumAsync(string id);
        Task AddMediaToAlbumAsync(string albumId, string mediaId);
        Task RemoveMediaFromAlbumAsync(string albumId, string mediaId);
    }
}