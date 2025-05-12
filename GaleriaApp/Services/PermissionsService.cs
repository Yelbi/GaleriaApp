namespace GaleriaApp.Services
{
    public interface IPermissionsService
    {
        Task<bool> EnsureStoragePermissionsAsync();
        Task<bool> EnsureCameraPermissionsAsync();
    }

    public class PermissionsService : IPermissionsService
    {
        public async Task<bool> EnsureStoragePermissionsAsync()
        {
            try
            {
                // En Android 13+ necesitamos permisos específicos
                if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Version.Major >= 13)
                {
                    var readImagesStatus = await Permissions.CheckStatusAsync<Permissions.Media>();
                    if (readImagesStatus != PermissionStatus.Granted)
                    {
                        readImagesStatus = await Permissions.RequestAsync<Permissions.Media>();
                    }

                    return readImagesStatus == PermissionStatus.Granted;
                }
                else
                {
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
                Console.WriteLine($"Error al solicitar permisos de almacenamiento: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnsureCameraPermissionsAsync()
        {
            try
            {
                var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                }

                return cameraStatus == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al solicitar permisos de cámara: {ex.Message}");
                return false;
            }
        }
    }
}