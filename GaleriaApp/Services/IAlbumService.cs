using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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