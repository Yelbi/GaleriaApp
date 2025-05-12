using GaleriaApp.Models;
using System.Text.Json;

namespace GaleriaApp.Services
{
    public class LocalStorageService : IStorageService
    {
        private const string MEDIA_FILE_KEY = "media_items.json";

        public async Task SaveMediaListAsync(List<MediaItem> mediaItems)
        {
            string appDataPath = FileSystem.AppDataDirectory;
            string filePath = Path.Combine(appDataPath, MEDIA_FILE_KEY);

            // Serializar la lista a JSON
            string jsonData = JsonSerializer.Serialize(mediaItems);

            // Guardar el archivo
            await File.WriteAllTextAsync(filePath, jsonData);
        }

        public async Task<List<MediaItem>> LoadMediaListAsync()
        {
            string appDataPath = FileSystem.AppDataDirectory;
            string filePath = Path.Combine(appDataPath, MEDIA_FILE_KEY);

            if (!File.Exists(filePath))
                return new List<MediaItem>();

            try
            {
                // Leer el archivo
                string jsonData = await File.ReadAllTextAsync(filePath);

                // Deserializar desde JSON
                var mediaItems = JsonSerializer.Deserialize<List<MediaItem>>(jsonData);

                // Filtrar archivos que ya no existen
                return mediaItems.Where(item => File.Exists(item.Path)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los medios: {ex.Message}");
                return new List<MediaItem>();
            }
        }

        public async Task RemoveMediaItemAsync(string id)
        {
            var mediaItems = await LoadMediaListAsync();
            var itemToRemove = mediaItems.FirstOrDefault(item => item.Id == id);

            if (itemToRemove != null)
            {
                mediaItems.Remove(itemToRemove);
                await SaveMediaListAsync(mediaItems);
            }
        }
    }
}