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

            // Si no, crear el thumbnail
            try
            {
                // Esta es una implementación simplificada, aquí iría el código real
                // para redimensionar la imagen y guardarla en caché
                using var originalImage = await LoadImageAsync(originalPath);
                using var thumbnail = ResizeImage(originalImage, size);
                await SaveImageAsync(thumbnail, thumbnailPath);

                return thumbnailPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando thumbnail: {ex.Message}");
                return originalPath; // En caso de error, devolver el original
            }
        }

        // Implementación de los demás métodos...
    }
}