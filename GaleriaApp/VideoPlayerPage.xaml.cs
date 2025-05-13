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
    private bool _isUserSeeking = false;
    private TimeSpan _totalDuration = TimeSpan.Zero;
    private Timer _hideControlsTimer;
    private bool _areControlsVisible = true;
    private double _currentPlaybackRate = 1.0;
    private double _currentVolume = 1.0;
    private bool _isMuted = false;

    // Evento para notificar cuando se elimina un elemento
    public event EventHandler<string>? MediaDeleted;

    public VideoPlayerPage(MediaItem mediaItem, IMediaService mediaService)
    {
        InitializeComponent();
        _mediaItem = mediaItem;
        _mediaService = mediaService;

        // Establecer el título
        Title = _mediaItem.Title;
        VideoTitleLabel.Text = _mediaItem.Title;

        // Inicializar controles
        InitializePlayer();

        // Configurar timer para ocultar controles
        _hideControlsTimer = new Timer(HideControlsCallback, null, Timeout.Infinite, Timeout.Infinite);

        // Agregar gestos para mostrar/ocultar controles
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnVideoTapped;
        ControlsOverlay.GestureRecognizers.Add(tapGesture);
    }

    private async void InitializePlayer()
    {
        try
        {
            LoadingOverlay.IsVisible = true;

            if (File.Exists(_mediaItem.Path))
            {
                // Configurar la fuente del video
                VideoPlayer.Source = MediaSource.FromFile(_mediaItem.Path);

                // Configurar propiedades iniciales
                VideoPlayer.Volume = _currentVolume;
                VideoPlayer.ShouldAutoPlay = false;

                // Actualizar información del video
                UpdateVideoInfo();
            }
            else
            {
                await DisplayAlert("Error", "No se puede cargar el video. El archivo no existe.", "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al inicializar el reproductor: {ex.Message}", "OK");
            await Navigation.PopAsync();
        }
    }

    private void UpdateVideoInfo()
    {
        try
        {
            var fileInfo = new FileInfo(_mediaItem.Path);
            var fileSizeMB = (fileInfo.Length / 1024.0 / 1024.0).ToString("F1");
            VideoInfoLabel.Text = $"{_mediaItem.DateCreated:dd/MM/yyyy} • {fileSizeMB} MB";
        }
        catch
        {
            VideoInfoLabel.Text = _mediaItem.DateCreated.ToString("dd/MM/yyyy");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Pausar y limpiar recursos
        VideoPlayer?.Stop();
        _hideControlsTimer?.Dispose();
    }

    #region Event Handlers - MediaElement

    private void OnMediaOpened(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = false;
        CenterPlayButton.IsVisible = true;

        _totalDuration = VideoPlayer.Duration;
        TotalTimeLabel.Text = FormatTimeSpan(_totalDuration);
        CurrentTimeLabel.Text = "00:00";
        ProgressSlider.Value = 0;

        // Auto-ocultar controles después de 3 segundos
        ResetHideControlsTimer();
    }

    private void OnMediaEnded(object sender, EventArgs e)
    {
        PlayPauseButton.Text = "▶";
        CenterPlayButton.Text = "▶";
        CenterPlayButton.IsVisible = true;

        // Mostrar controles al final
        ShowControls();
    }

    private void OnMediaFailed(object sender, MediaFailedEventArgs e)
    {
        LoadingOverlay.IsVisible = false;
        Device.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert("Error", $"No se pudo reproducir el video: {e.ErrorMessage}", "OK");
        });
    }

    private void OnStateChanged(object sender, MediaStateChangedEventArgs e)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            switch (e.NewState)
            {
                case MediaElementState.Playing:
                    PlayPauseButton.Text = "⏸";
                    CenterPlayButton.Text = "⏸";
                    CenterPlayButton.IsVisible = false;
                    break;
                case MediaElementState.Paused:
                case MediaElementState.Stopped:
                    PlayPauseButton.Text = "▶";
                    CenterPlayButton.Text = "▶";
                    CenterPlayButton.IsVisible = true;
                    ShowControls();
                    break;
                case MediaElementState.Buffering:
                    LoadingOverlay.IsVisible = true;
                    break;
            }
        });
    }

    private void OnPositionChanged(object sender, MediaPositionChangedEventArgs e)
    {
        if (!_isUpdatingSlider && !_isUserSeeking && _totalDuration.TotalSeconds > 0)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                double progress = e.Position.TotalSeconds / _totalDuration.TotalSeconds * 100;
                ProgressSlider.Value = progress;
                CurrentTimeLabel.Text = FormatTimeSpan(e.Position);
            });
        }
    }

    #endregion

    #region Control Event Handlers

    private void OnVideoTapped(object sender, TappedEventArgs e)
    {
        ToggleControlsVisibility();
        ResetHideControlsTimer();
    }

    private void OnCenterPlayClicked(object sender, EventArgs e)
    {
        OnPlayPauseClicked(sender, e);
    }

    private void OnPlayPauseClicked(object sender, EventArgs e)
    {
        if (VideoPlayer.CurrentState == MediaElementState.Playing)
        {
            VideoPlayer.Pause();
        }
        else
        {
            VideoPlayer.Play();

            // Si llegó al final, reiniciar
            if (VideoPlayer.Position >= _totalDuration)
            {
                VideoPlayer.SeekTo(TimeSpan.Zero);
            }
        }

        ResetHideControlsTimer();
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
        ResetHideControlsTimer();
    }

    private void OnForwardClicked(object sender, EventArgs e)
    {
        TimeSpan newPosition;
        if (VideoPlayer.Position.TotalSeconds + 10 < _totalDuration.TotalSeconds)
        {
            newPosition = VideoPlayer.Position + TimeSpan.FromSeconds(10);
        }
        else
        {
            newPosition = _totalDuration;
        }
        VideoPlayer.SeekTo(newPosition);
        ResetHideControlsTimer();
    }

    private void OnProgressDragStarted(object sender, EventArgs e)
    {
        _isUserSeeking = true;
        _hideControlsTimer?.Change(Timeout.Infinite, Timeout.Infinite);
    }

    private void OnProgressChanged(object sender, ValueChangedEventArgs e)
    {
        if (_isUserSeeking && _totalDuration.TotalSeconds > 0)
        {
            var newPosition = TimeSpan.FromSeconds(e.NewValue / 100 * _totalDuration.TotalSeconds);
            CurrentTimeLabel.Text = FormatTimeSpan(newPosition);
        }
    }

    private void OnProgressDragCompleted(object sender, EventArgs e)
    {
        if (_totalDuration.TotalSeconds > 0)
        {
            var newPosition = TimeSpan.FromSeconds(ProgressSlider.Value / 100 * _totalDuration.TotalSeconds);
            VideoPlayer.SeekTo(newPosition);
        }

        _isUserSeeking = false;
        ResetHideControlsTimer();
    }

    private async void OnSpeedClicked(object sender, EventArgs e)
    {
        // Cambiar velocidad de reproducción
        var rates = new[] { 0.25, 0.5, 0.75, 1.0, 1.25, 1.5, 2.0 };
        var current = rates.FirstOrDefault(r => Math.Abs(r - _currentPlaybackRate) < 0.01);
        var currentIndex = Array.IndexOf(rates, current);
        var nextIndex = (currentIndex + 1) % rates.Length;

        _currentPlaybackRate = rates[nextIndex];
        VideoPlayer.Speed = _currentPlaybackRate;
        SpeedButton.Text = $"{_currentPlaybackRate}x";

        ResetHideControlsTimer();
    }

    private void OnVolumeClicked(object sender, EventArgs e)
    {
        _isMuted = !_isMuted;

        if (_isMuted)
        {
            VideoPlayer.Volume = 0;
            VolumeButton.Text = "🔇";
        }
        else
        {
            VideoPlayer.Volume = _currentVolume;
            VolumeButton.Text = "🔊";
        }

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
                VideoPlayer.Stop();
                MediaDeleted?.Invoke(this, _mediaItem.Id);
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo eliminar el video: {ex.Message}", "OK");
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
            FullScreenButton.Text = "⛷";
            FullScreenTopButton.Text = "⛷";
        }
        else
        {
            // Salir de pantalla completa
            Shell.SetNavBarIsVisible(this, true);
            Shell.SetTabBarIsVisible(this, true);
            Title = _mediaItem.Title;
            FullScreenButton.Text = "⛶";
            FullScreenTopButton.Text = "⛶";
        }

        ResetHideControlsTimer();
    }

    #endregion

    #region Helper Methods

    private void ToggleControlsVisibility()
    {
        if (_areControlsVisible)
        {
            HideControls();
        }
        else
        {
            ShowControls();
        }
    }

    private async void ShowControls()
    {
        _areControlsVisible = true;

        // Animar mostrar controles
        BottomControls.IsVisible = true;
        ControlsOverlay.IsVisible = true;

        await Task.WhenAll(
            BottomControls.FadeTo(1, 200),
            ControlsOverlay.FadeTo(1, 200)
        );
    }

    private async void HideControls()
    {
        _areControlsVisible = false;

        // Animar ocultar controles
        await Task.WhenAll(
            BottomControls.FadeTo(0, 200),
            ControlsOverlay.FadeTo(0, 200)
        );

        BottomControls.IsVisible = false;
        ControlsOverlay.IsVisible = false;
    }

    private void ResetHideControlsTimer()
    {
        _hideControlsTimer?.Change(3000, Timeout.Infinite);
    }

    private void HideControlsCallback(object state)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            if (_areControlsVisible && VideoPlayer.CurrentState == MediaElementState.Playing)
            {
                HideControls();
            }
        });
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return timeSpan.Hours > 0
            ? $"{timeSpan.Hours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}"
            : $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    #endregion
}