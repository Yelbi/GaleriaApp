using CommunityToolkit.Maui;
using GaleriaApp.Services;
using GaleriaApp.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;

namespace GaleriaApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Registrar servicios
        builder.Services.AddSingleton<IMediaService, MediaService>();
        builder.Services.AddSingleton<IStorageService, LocalStorageService>();
        builder.Services.AddSingleton<IImageCacheService, ImageCacheService>();
        builder.Services.AddSingleton<IPermissionsService, PermissionsService>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();

        // Registrar ViewModels
        builder.Services.AddSingleton<MainViewModel>();

        // Registrar páginas
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<ImageDetailPage>();
        builder.Services.AddTransient<VideoPlayerPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        try
        {
            return builder.Build();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error building MAUI app: {ex}");
            throw;
        }
    }
}