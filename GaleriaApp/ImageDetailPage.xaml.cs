using GaleriaApp.Models;
using GaleriaApp.Services;
using System.IO;

namespace GaleriaApp;

public partial class ImageDetailPage : ContentPage
{
    private readonly MediaItem _mediaItem;
    private readonly IMediaService _mediaService;

    // Propiedades para gestionar los gestos
    private double _currentScale = 1;
    private double _startScale = 1;
    private double _currentX = 0;
    private double _currentY = 0;
    private double _xOffset = 0;
    private double _yOffset = 0;
    private double _currentRotation = 0;

    private bool _isControlsVisible = true;
    private Timer _hideControlsTimer;

    // Evento para notificar cuando se elimina un elemento
    public event EventHandler<string>? MediaDeleted;

    public ImageDetailPage(MediaItem mediaItem, IMediaService mediaService)
    {
        InitializeComponent();
        _mediaItem = mediaItem;
        _mediaService = mediaService;

        // Configurar la página
        Title = _mediaItem.Title;
        ImageTitleLabel.Text = _mediaItem.Title;

        // Cargar la imagen
        LoadImageAsync();

        // Configurar información de la imagen
        UpdateImageInfo();

        // Configurar timer para ocultar controles
        _hideControlsTimer = new Timer(HideControlsCallback, null, Timeout.Infinite, Timeout.Infinite);
    }

    private async Task LoadImageAsync()
    {
        try
        {
            LoadingOverlay.IsVisible = true;

            if (File.Exists(_mediaItem.Path))
            {
                // Cargar imagen
                DetailImage.Source = ImageSource.FromFile(_mediaItem.Path);

                // Esperar un poco para que la imagen se cargue
                await Task.Delay(500);

                UpdateImageInfo();
            }
            else
            {
                await DisplayAlert("Error", "No se puede cargar la imagen. El archivo no existe.", "OK");
                await Navigation.PopAsync();
                return;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar la imagen: {ex.Message}", "OK");
            await Navigation.PopAsync();
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

    private void UpdateImageInfo()
    {
        try
        {
            var fileInfo = new FileInfo(_mediaItem.Path);
            var fileSizeMB = (fileInfo.Length / 1024.0 / 1024.0).ToString("F1");
            ImageInfoLabel.Text = $"{_mediaItem.DateCreated:dd/MM/yyyy} • {fileSizeMB} MB";
        }
        catch
        {
            ImageInfoLabel.Text = _mediaItem.DateCreated.ToString("dd/MM/yyyy");
        }
    }

    private void OnSingleTap(object sender, TappedEventArgs e)
    {
        // Alternar visibilidad de controles
        ToggleControlsVisibility();
        ResetHideControlsTimer();
    }

    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        // Zoom inteligente
        if (_currentScale > 1)
        {
            // Volver a escala normal
            _currentScale = 1;
            DetailImage.Scale = _currentScale;
            DetailImage.TranslationX = 0;
            DetailImage.TranslationY = 0;
            _xOffset = 0;
            _yOffset = 0;
        }
        else
        {
            // Ampliar a escala 2x
            _currentScale = 2;
            DetailImage.Scale = _currentScale;
        }

        ResetHideControlsTimer();
    }

    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                _startScale = _currentScale;
                break;

            case GestureStatus.Running:
                _currentScale = Math.Max(_startScale * e.Scale, 0.5);
                _currentScale = Math.Clamp(_currentScale, 0.5, 5.0);
                DetailImage.Scale = _currentScale;
                break;

            case GestureStatus.Completed:
                if (_currentScale < 0.8)
                {
                    _currentScale = 1;
                    DetailImage.Scale = 1;
                    DetailImage.TranslationX = 0;
                    DetailImage.TranslationY = 0;
                    _xOffset = 0;
                    _yOffset = 0;
                }
                break;
        }
        ResetHideControlsTimer();
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                if (_currentScale > 1)
                {
                    _currentX = _xOffset + e.TotalX;
                    _currentY = _yOffset + e.TotalY;

                    // Aplicar límites para evitar que la imagen se salga demasiado
                    var maxX = (DetailImage.Width * _currentScale - DetailImage.Width) / 2;
                    var maxY = (DetailImage.Height * _currentScale - DetailImage.Height) / 2;

                    _currentX = Math.Clamp(_currentX, -maxX, maxX);
                    _currentY = Math.Clamp(_currentY, -maxY, maxY);

                    DetailImage.TranslationX = _currentX;
                    DetailImage.TranslationY = _currentY;
                }
                break;

            case GestureStatus.Completed:
                _xOffset = _currentX;
                _yOffset = _currentY;
                break;
        }
        ResetHideControlsTimer();
    }

    private void ToggleControlsVisibility()
    {
        _isControlsVisible = !_isControlsVisible;

        var fadeTask = _isControlsVisible
            ? ControlsOverlay.FadeTo(1, 200)
            : ControlsOverlay.FadeTo(0, 200);

        fadeTask.ContinueWith(_ =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ControlsOverlay.IsVisible = _isControlsVisible;
            });
        });
    }

    private void ResetHideControlsTimer()
    {
        _hideControlsTimer?.Change(3000, Timeout.Infinite);
    }

    private void HideControlsCallback(object state)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            if (_isControlsVisible)
            {
                ToggleControlsVisibility();
            }
        });
    }

    private void OnRotateLeftClicked(object sender, EventArgs e)
    {
        _currentRotation = (_currentRotation - 90) % 360;
        DetailImage.Rotation = _currentRotation;
        ResetHideControlsTimer();
    }

    private void OnRotateRightClicked(object sender, EventArgs e)
    {
        _currentRotation = (_currentRotation + 90) % 360;
        DetailImage.Rotation = _currentRotation;
        ResetHideControlsTimer();
    }

    private void OnFitToScreenClicked(object sender, EventArgs e)
    {
        // Resetear zoom y rotación
        _currentScale = 1;
        DetailImage.Scale = 1;
        DetailImage.TranslationX = 0;
        DetailImage.TranslationY = 0;
        DetailImage.Rotation = 0;
        _currentRotation = 0;
        _xOffset = 0;
        _yOffset = 0;
        ResetHideControlsTimer();
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
            ResetHideControlsTimer();
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
                MediaDeleted?.Invoke(this, _mediaItem.Id);
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo eliminar la imagen: {ex.Message}", "OK");
            }
        }
    }

    private void OnFullScreenClicked(object sender, EventArgs e)
    {
        var isFullscreen = Shell.GetNavBarIsVisible(this);

        if (isFullscreen)
        {
            // Activar pantalla completa
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetTabBarIsVisible(this, false);
            Title = string.Empty;
            FullScreenButton.Text = "⛶";
        }
        else
        {
            // Salir de pantalla completa
            Shell.SetNavBarIsVisible(this, true);
            Shell.SetTabBarIsVisible(this, true);
            Title = _mediaItem.Title;
            FullScreenButton.Text = "⛷";
        }

        ResetHideControlsTimer();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _hideControlsTimer?.Dispose();
    }
}