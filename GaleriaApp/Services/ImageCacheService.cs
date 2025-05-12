using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Nuevo archivo: Services/ImageCacheService.cs
namespace GaleriaApp.Services
{
    public interface IImageCacheService
    {
        Task<string> GetThumbnailPathAsync(string originalPath, int size = 200);
        Task ClearCacheAsync();
        Task PreloadThumbnailsAsync(IEnumerable<string> imagePaths);
    }

    public class ImageCacheService : IImageCacheService
    {
        private readonly string _cacheFolder;

        public ImageCacheService()
        {
            _cacheFolder = Path.Combine(FileSystem.CacheDirectory, "ImageThumbnails");
            if (!Directory.Exists(_cacheFolder))
                Directory.CreateDirectory(_cacheFolder);
        }

        public async Task<string> GetThumbnailPathAsync(string originalPath, int size = 200)
        {
            // Crear un nombre de archivo único basado en el path original y tamaño
            string filename = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);
            string thumbnailName = $"{filename}_{size}{extension}";
            string thumbnailPath = Path.Combine(_cacheFolder, thumbnailName);

            // Si el thumbnail ya existe, devolverlo directamente
            if (File.Exists(thumbnailPath))
                return thumbnailPath;

            // En una implementación real, aquí se crearía el thumbnail
            // Por ahora, simplemente devolvemos el original para evitar errores
            return originalPath;
        }

        public async Task ClearCacheAsync()
        {
            try
            {
                if (Directory.Exists(_cacheFolder))
                {
                    var files = Directory.GetFiles(_cacheFolder);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al limpiar caché: {ex.Message}");
            }
        }

        public async Task PreloadThumbnailsAsync(IEnumerable<string> imagePaths)
        {
            foreach (var path in imagePaths)
            {
                await GetThumbnailPathAsync(path);
            }
        }
    }
}