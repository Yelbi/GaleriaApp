namespace GaleriaApp.Services
{
    public interface IImageEditorService
    {
        Task<string> RotateImageAsync(string imagePath, int degrees);
        Task<string> ApplyFilterAsync(string imagePath, string filterType);
        Task<string> CropImageAsync(string imagePath, Rect cropRect);
        Task<string> AdjustBrightnessContrastAsync(string imagePath, float brightness, float contrast);
    }
}