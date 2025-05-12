using CommunityToolkit.Maui;
using GaleriaApp.Services;
using GaleriaApp.ViewModels; // Añadir este using
using Microsoft.Extensions.Logging;

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
                // Agregar fuente de FontAwesome si la vas a usar
                fonts.AddFont("FontAwesome.ttf", "FontAwesome");
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

        return builder.Build();
    }
}