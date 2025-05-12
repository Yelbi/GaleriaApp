using GaleriaApp.Models;
using GaleriaApp.Services;

namespace GaleriaApp;

public partial class ImageDetailPage : ContentPage
{
    private readonly MediaItem _mediaItem;
    private readonly IMediaService _mediaService;

    // Propiedades para gestionar los gestos
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
            DetailImage.Source = ImageSource.FromFile(_mediaItem.Path);
        }
        else
        {
            // Si la imagen no existe, mostrar un mensaje y volver atrás
            DisplayAlert("Error", "No se puede cargar la imagen. El archivo no existe.", "OK");
            Navigation.PopAsync();
        }

        // Establecer el título
        Title = _mediaItem.Title;

        // Suscribirse al evento de carga de la imagen
        DetailImage.SizeChanged += OnImageLoaded;
    }

    private void OnImageLoaded(object sender, EventArgs e)
    {
        // Ocultar el indicador de carga cuando la imagen se ha cargado
        LoadingIndicator.IsVisible = false;
        LoadingIndicator.IsRunning = false;

        // Desuscribirse del evento para evitar múltiples llamadas
        DetailImage.SizeChanged -= OnImageLoaded;
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        try
        {
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = _mediaItem.Title,
                File = new ShareFile(_mediaItem.Path)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo compartir la imagen: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirmar eliminación",
            "¿Estás seguro de que quieres eliminar esta imagen?",
            "Sí", "No");

        if (confirm)
        {
            try
            {
                // Notificar a la página principal que se eliminó este elemento
                MediaDeleted?.Invoke(this, _mediaItem.Id);

                // Regresar a la página anterior
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo eliminar la imagen: {ex.Message}", "OK");
            }
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
                _currentScale = Math.Max(_startScale * e.Scale, 0.5);

                // Limitar la escala entre valores razonables
                _currentScale = Math.Clamp(_currentScale, 0.5, 5.0);

                // Aplicar la escala a la imagen
                DetailImage.Scale = _currentScale;
                break;

            case GestureStatus.Completed:
                // Si la escala es muy pequeña, resetear al tamaño original
                if (_currentScale < 0.8)
                {
                    _currentScale = 1;
                    DetailImage.Scale = 1;
                }
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
            DetailImage.TranslationX = 0;
            DetailImage.TranslationY = 0;
        }
        else
        {
            // Ampliar a escala 2x
            _currentScale = 2;
            DetailImage.Scale = 2;
        }
    }

    // Método para rotación
    private void OnRotateClicked(object sender, EventArgs e)
    {
        _currentRotation = (_currentRotation + 90) % 360;
        DetailImage.Rotation = _currentRotation;
    }

    // Implementar método para pantalla completa
    private void OnFullScreenClicked(object sender, EventArgs e)
    {
        bool isFullscreen = Shell.GetNavBarIsVisible(this);

        if (isFullscreen)
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