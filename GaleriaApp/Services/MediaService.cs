using GaleriaApp.Models;
using Microsoft.Maui.Storage;

namespace GaleriaApp.Services
{
    public class MediaService : IMediaService
    {
        public async Task<List<MediaItem>> GetMediaFromDeviceAsync()
        {
            // Comprobar y solicitar permisos
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                    return new List<MediaItem>();
            }

            // En una aplicación real, aquí consultarías la galería del dispositivo
            // usando APIs específicas de la plataforma
            // Por ahora, devolvemos una lista vacía que llenaremos con los archivos
            // que el usuario seleccione manualmente
            return new List<MediaItem>();
        }

        public async Task<FileResult?> PickPhotoAsync() // Cambia el tipo de retorno
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                return photo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al seleccionar foto: {ex.Message}");
                return null;
            }
        }

        public async Task<FileResult?> PickVideoAsync() // Cambia el tipo de retorno
        {
            try
            {
                var video = await MediaPicker.PickVideoAsync();
                return video;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al seleccionar video: {ex.Message}");
                return null;
            }
        }
    }
}