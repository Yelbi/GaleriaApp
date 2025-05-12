using CommunityToolkit.Maui;
using GaleriaApp.Services;
using Microsoft.Extensions.Logging;
namespace GaleriaApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // Usa esto en lugar de UseMauiCommunityToolkitCore
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Registrar servicios y páginas
        builder.Services.AddSingleton<IMediaService, MediaService>();
        builder.Services.AddSingleton<IStorageService, LocalStorageService>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<ImageDetailPage>();
        builder.Services.AddTransient<VideoPlayerPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}