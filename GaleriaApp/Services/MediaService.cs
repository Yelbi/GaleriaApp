using GaleriaApp.Models;

namespace GaleriaApp.Services
{
    public class MediaService : IMediaService
    {
        public async Task<List<MediaItem>> GetMediaFromDeviceAsync()
        {
            // Comprobar y solicitar permisos
            var status = await CheckAndRequestPermissions();
            if (!status)
                return new List<MediaItem>();

            // En una aplicación real, aquí consultarías la galería del dispositivo
            // usando APIs específicas de la plataforma
            return new List<MediaItem>();
        }

        public async Task<FileResult?> PickPhotoAsync()
        {
            try
            {
                // Verificar permisos
                var hasPermissions = await CheckAndRequestPermissions();
                if (!hasPermissions)
                {
                    return null;
                }

                // Configurar opciones de selección
                var options = new PickOptions
                {
                    PickerTitle = "Seleccionar una imagen",
                    FileTypes = FilePickerFileType.Images
                };

                var photo = await FilePicker.PickAsync(options);

                // Verificar si es una imagen válida
                if (photo != null && IsValidImageFile(photo.FileName))
                {
                    return photo;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al seleccionar foto: {ex.Message}");
                return null;
            }
        }

        public async Task<FileResult?> PickVideoAsync()
        {
            try
            {
                // Verificar permisos
                var hasPermissions = await CheckAndRequestPermissions();
                if (!hasPermissions)
                {
                    return null;
                }

                // Configurar opciones de selección
                var options = new PickOptions
                {
                    PickerTitle = "Seleccionar un video",
                    FileTypes = FilePickerFileType.Videos
                };

                var video = await FilePicker.PickAsync(options);

                // Verificar si es un video válido
                if (video != null && IsValidVideoFile(video.FileName))
                {
                    return video;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al seleccionar video: {ex.Message}");
                return null;
            }
        }

        // Método para tomar fotos con la cámara
        public async Task<FileResult?> TakePhotoAsync()
        {
            try
            {
                // Verificar permisos de cámara
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                        return null;
                }

                var photo = await MediaPicker.CapturePhotoAsync();
                return photo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al tomar foto: {ex.Message}");
                return null;
            }
        }

        // Método para grabar videos con la cámara
        public async Task<FileResult?> CaptureVideoAsync()
        {
            try
            {
                // Verificar permisos de cámara
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                        return null;
                }

                var video = await MediaPicker.CaptureVideoAsync(new MediaPickerOptions
                {
                    Title = "Grabar video"
                });
                return video;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al grabar video: {ex.Message}");
                return null;
            }
        }

        private async Task<bool> CheckAndRequestPermissions()
        {
            try
            {
                // Para Android 13+ (API 33+) necesitamos permisos específicos
                if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Version.Major >= 13)
                {
                    var readImagesStatus = await Permissions.CheckStatusAsync<Permissions.Photos>();
                    if (readImagesStatus != PermissionStatus.Granted)
                    {
                        readImagesStatus = await Permissions.RequestAsync<Permissions.Photos>();
                    }

                    var readMediaStatus = await Permissions.CheckStatusAsync<Permissions.Media>();
                    if (readMediaStatus != PermissionStatus.Granted)
                    {
                        readMediaStatus = await Permissions.RequestAsync<Permissions.Media>();
                    }

                    return readImagesStatus == PermissionStatus.Granted && readMediaStatus == PermissionStatus.Granted;
                }
                else
                {
                    // Para versiones anteriores
                    var readStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                    if (readStatus != PermissionStatus.Granted)
                    {
                        readStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
                    }

                    return readStatus == PermissionStatus.Granted;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar permisos: {ex.Message}");
                return false;
            }
        }

        private bool IsValidImageFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

            return validExtensions.Contains(extension);
        }

        private bool IsValidVideoFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var validExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm", ".mkv", ".m4v" };

            return validExtensions.Contains(extension);
        }

        // Método para obtener información de un archivo multimedia
        public async Task<MediaItem?> GetMediaItemInfoAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                var fileInfo = new FileInfo(filePath);
                var extension = fileInfo.Extension.ToLowerInvariant();

                string mediaType;
                if (IsValidImageFile(filePath))
                {
                    mediaType = "Image";
                }
                else if (IsValidVideoFile(filePath))
                {
                    mediaType = "Video";
                }
                else
                {
                    return null;
                }

                return new MediaItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = Path.GetFileNameWithoutExtension(fileInfo.Name),
                    Path = filePath,
                    Type = mediaType,
                    DateCreated = fileInfo.CreationTime
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener información del archivo: {ex.Message}");
                return null;
            }
        }
    }
}