using GaleriaApp.Models;
using GaleriaApp.Services;

namespace GaleriaApp;

public partial class ImageDetailPage : ContentPage
{
    private readonly MediaItem _mediaItem;
    private readonly IMediaService _mediaService;

    // Evento para notificar cuando se elimina un elemento
    public event EventHandler<string>? MediaDeleted;

    public ImageDetailPage(MediaItem mediaItem, IMediaService mediaService)
    {
        InitializeComponent();
        _mediaItem = mediaItem;
        _mediaService = mediaService;

        // Cargar la imagen
        if (File.Exists(_mediaItem.Path))
        {
            DetailImage.Source = _mediaItem.Path;
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
            "¿Estás seguro de que quieres eliminar esta imagen?",
            "Sí", "No");

        if (confirm)
        {
            // Notificar a la página principal que se eliminó este elemento
            MediaDeleted?.Invoke(this, _mediaItem.Id);

            // Regresar a la página anterior
            await Navigation.PopAsync();
        }
    }
}