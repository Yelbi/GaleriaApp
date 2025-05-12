using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using GaleriaApp.Models;
using GaleriaApp.Services;

namespace GaleriaApp;

public partial class VideoPlayerPage : ContentPage
{
    private readonly MediaItem _mediaItem;
    private readonly IMediaService _mediaService;
    private bool _isUpdatingSlider = false;
    private TimeSpan _totalDuration = TimeSpan.Zero;

    // Evento para notificar cuando se elimina un elemento
    public event EventHandler<string>? MediaDeleted;

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
        else
        {
            // Si el video no existe, mostrar un mensaje y volver atrás
            DisplayAlert("Error", "No se puede cargar el video. El archivo no existe.", "OK");
            Navigation.PopAsync();
        }

        // Establecer el título
        Title = _mediaItem.Title;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Detener el video al salir de la página
        VideoPlayer.Stop();
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
            await DisplayAlert("Error", $"No se pudo compartir el video: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirmar eliminación",
            "¿Estás seguro de que quieres eliminar este video?",
            "Sí", "No");

        if (confirm)
        {
            try
            {
                // Detener el video antes de eliminarlo
                VideoPlayer.Stop();

                // Notificar a la página principal que se eliminó este elemento
                MediaDeleted?.Invoke(this, _mediaItem.Id);

                // Regresar a la página anterior
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo eliminar el video: {ex.Message}", "OK");
            }
        }
    }

    // Manejadores para el reproductor
    private void OnMediaOpened(object sender, EventArgs e)
    {
        _totalDuration = VideoPlayer.Duration;
        TotalTimeLabel.Text = FormatTimeSpan(_totalDuration);
        CurrentTimeLabel.Text = "00:00";
        ProgressSlider.Value = 0;
    }

    private void OnMediaEnded(object sender, EventArgs e)
    {
        PlayPauseButton.Text = "Reproducir";
    }

    private void OnMediaFailed(object sender, MediaFailedEventArgs e)
    {
        DisplayAlert("Error", $"No se pudo reproducir el video: {e.ErrorMessage}", "OK");
    }

    private void OnPositionChanged(object sender, MediaPositionChangedEventArgs e)
    {
        if (!_isUpdatingSlider && _totalDuration.TotalSeconds > 0)
        {
            double progress = e.Position.TotalSeconds / _totalDuration.TotalSeconds;
            ProgressSlider.Value = progress;
            CurrentTimeLabel.Text = FormatTimeSpan(e.Position);
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (_totalDuration.TotalSeconds > 0 && Math.Abs(e.OldValue - e.NewValue) > 0.01)
        {
            _isUpdatingSlider = true;
            TimeSpan newPosition = TimeSpan.FromSeconds(e.NewValue * _totalDuration.TotalSeconds);
            VideoPlayer.SeekTo(newPosition);
            CurrentTimeLabel.Text = FormatTimeSpan(newPosition);
            _isUpdatingSlider = false;
        }
    }

    private void OnPlayPauseClicked(object sender, EventArgs e)
    {
        if (VideoPlayer.CurrentState == MediaElementState.Playing)
        {
            VideoPlayer.Pause();
            PlayPauseButton.Text = "Reproducir";
        }
        else
        {
            VideoPlayer.Play();
            PlayPauseButton.Text = "Pausa";

            // Si llegó al final, reiniciar
            if (VideoPlayer.Position >= _totalDuration)
            {
                VideoPlayer.SeekTo(TimeSpan.Zero);
            }
        }
    }

    private void OnRewindClicked(object sender, EventArgs e)
    {
        TimeSpan newPosition;
        if (VideoPlayer.Position.TotalSeconds >= 10)
        {
            newPosition = VideoPlayer.Position - TimeSpan.FromSeconds(10);
        }
        else
        {
            newPosition = TimeSpan.Zero;
        }
        VideoPlayer.SeekTo(newPosition);
    }

    private void OnForwardClicked(object sender, EventArgs e)
    {
        if (VideoPlayer.Position.TotalSeconds + 10 < _totalDuration.TotalSeconds)
        {
            TimeSpan newPosition = VideoPlayer.Position + TimeSpan.FromSeconds(10);
            VideoPlayer.SeekTo(newPosition);
        }
        else
        {
            // Si estamos casi al final, ir al final
            VideoPlayer.SeekTo(_totalDuration);
        }
    }

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

    // Método helper para formatear tiempo
    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return timeSpan.Hours > 0
            ? $"{timeSpan.Hours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}"
            : $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
}