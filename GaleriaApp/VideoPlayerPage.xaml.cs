using GaleriaApp.Models;
using GaleriaApp.Services;
using CommunityToolkit.Maui.Core; // Añade esto
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views; // Añade esto para MediaSource

namespace GaleriaApp;

public partial class VideoPlayerPage : ContentPage
{
    private readonly MediaItem _mediaItem;
    private readonly IMediaService _mediaService;

    // Evento para notificar cuando se elimina un elemento
    public event EventHandler<string> MediaDeleted;

    public VideoPlayerPage(MediaItem mediaItem, IMediaService mediaService)
    {
        InitializeComponent();
        _mediaItem = mediaItem;
        _mediaService = mediaService;

        // Cargar el video
        if (File.Exists(_mediaItem.Path))
        {
            VideoPlayer.Source = MediaSource.FromFile(_mediaItem.Path);
        }

        // Establecer el título
        Title = _mediaItem.Title;
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        await Share.RequestAsync(new ShareFileRequest
        {
            Title = _mediaItem.Title,
            File = new ShareFile(_mediaItem.Path)
        });
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirmar eliminación",
            "¿Estás seguro de que quieres eliminar este video?",
            "Sí", "No");

        if (confirm)
        {
            // Notificar a la página principal que se eliminó este elemento
            MediaDeleted?.Invoke(this, _mediaItem.Id);

            // Regresar a la página anterior
            await Navigation.PopAsync();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Detener el video al salir de la página
        VideoPlayer.Stop();
    }
}