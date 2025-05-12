using GaleriaApp.Models;
using GaleriaApp.Services;

namespace GaleriaApp;

public partial class ImageDetailPage : ContentPage
{
    private readonly MediaItem _mediaItem;
    private readonly IMediaService _mediaService;
    private bool _isFullscreen = false;
    private float _currentScale = 1;
    private float _startScale = 1;
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

                // Limitar la escala entre los valores mínimo y máximo
                _currentScale = Math.Clamp(_currentScale, (float)ImageScrollView.MinimumZoomScale, (float)ImageScrollView.MaximumZoomScale);

                // Aplicar la escala a la imagen
                ImageScrollView.ZoomTo(_currentScale, e.ScaleOrigin.X, e.ScaleOrigin.Y, false);
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
            ImageScrollView.ZoomTo(1, true);
        }
        else
        {
            // Ampliar a escala 2x
            _currentScale = 2;
            ImageScrollView.ZoomTo(2, 0.5, 0.5, true);
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
}s