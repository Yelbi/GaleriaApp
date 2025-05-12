using GaleriaApp.Services;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.MediaElement;

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

        // Registrar el servicio
        builder.Services.AddSingleton<IMediaService, MediaService>();

        // Registrar la página principal
        builder.Services.AddSingleton<MainPage>();

        // Añadir a las líneas existentes de registro de servicios
        builder.Services.AddSingleton<IStorageService, LocalStorageService>();

        // Registrar las nuevas páginas
        builder.Services.AddTransient<ImageDetailPage>();
        builder.Services.AddTransient<VideoPlayerPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}