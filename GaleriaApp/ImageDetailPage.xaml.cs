using GaleriaApp.Models;
using GaleriaApp.Services;

namespace GaleriaApp;

public partial class ImageDetailPage : ContentPage
{
    private readonly MediaItem _mediaItem;
    private readonly IMediaService _mediaService;
    private bool _isFullscreen = false;
    private double _currentScale = 1;
    private double _startScale = 1;
    private double _currentRotation = 0;

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

    // Manejador del gesto de pellizco para zoom
    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                _startScale = _currentScale;
                break;
            case GestureStatus.Running:
                // Calcular la nueva escala basada en el factor de pinch
                _currentScale = _startScale * e.Scale;

                // Limitar la escala entre valores razonables
                _currentScale = Math.Clamp(_currentScale, 0.5, 4.0);

                // Aplicar la escala directamente a la imagen
                DetailImage.Scale = _currentScale;
                break;
        }
    }

    // Manejador para doble toque (zoom rápido)
    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        if (_currentScale > 1)
        {
            // Si está ampliado, volver a escala normal
            _currentScale = 1;
            DetailImage.Scale = 1;
        }
        else
        {
            // Ampliar a escala 2x
            _currentScale = 2;
            DetailImage.Scale = 2;
        }
    }

    // Nuevo método para rotación
    private void OnRotateClicked(object sender, EventArgs e)
    {
        _currentRotation = (_currentRotation + 90) % 360;
        DetailImage.Rotation = _currentRotation;
    }

    // Nuevo método para pantalla completa
    private void OnFullScreenClicked(object sender, EventArgs e)
    {
        _isFullscreen = !_isFullscreen;

        if (_isFullscreen)
        {
            // Ocultar elementos de navegación
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetTabBarIsVisible(this, false);
            Title = string.Empty;
        }
        else
        {
            // Mostrar elementos de navegación
            Shell.SetNavBarIsVisible(this, true);
            Shell.SetTabBarIsVisible(this, true);
            Title = _mediaItem.Title;
        }
    }
}