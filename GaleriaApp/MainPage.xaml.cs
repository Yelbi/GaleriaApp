using GaleriaApp.Models;
using GaleriaApp.Services;

namespace GaleriaApp;

public partial class MainPage : ContentPage
{
    private readonly IMediaService _mediaService;
    private readonly IStorageService _storageService;
    private List<MediaItem> _mediaItems = new List<MediaItem>();

    public MainPage(IMediaService mediaService, IStorageService storageService)
    {
        InitializeComponent();
        _mediaService = mediaService;
        _storageService = storageService;

        // Cargar medios guardados al iniciar
        LoadSavedMediaAsync();
    }

    private async Task LoadSavedMediaAsync()
    {
        _mediaItems = await _storageService.LoadMediaListAsync();
        MediaCollection.ItemsSource = _mediaItems;
    }

    private async void OnSelectPhotoClicked(object sender, EventArgs e)
    {
        var photo = await _mediaService.PickPhotoAsync();
        if (photo != null)
        {
            // Añadir a nuestra colección
            var newItem = new MediaItem
            {
                Id = Guid.NewGuid().ToString(),
                Title = Path.GetFileName(photo.FileName),
                Path = photo.FullPath,
                Type = "Image",
                DateCreated = DateTime.Now
            };

            _mediaItems.Add(newItem);
            RefreshMediaCollection();

            // Guardar cambios
            await _storageService.SaveMediaListAsync(_mediaItems);
        }
    }

    private async void OnSelectVideoClicked(object sender, EventArgs e)
    {
        var video = await _mediaService.PickVideoAsync();
        if (video != null)
        {
            // Añadir a nuestra colección
            var newItem = new MediaItem
            {
                Id = Guid.NewGuid().ToString(),
                Title = Path.GetFileName(video.FileName),
                Path = video.FullPath,
                Type = "Video",
                DateCreated = DateTime.Now
            };

            _mediaItems.Add(newItem);
            RefreshMediaCollection();

            // Guardar cambios
            await _storageService.SaveMediaListAsync(_mediaItems);
        }
    }

    private void RefreshMediaCollection()
    {
        MediaCollection.ItemsSource = null;
        MediaCollection.ItemsSource = _mediaItems;
    }

    private async void OnMediaSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is MediaItem selectedItem)
        {
            // Dependiendo del tipo, navegar a la página correspondiente
            if (selectedItem.Type == "Image")
            {
                var detailPage = new ImageDetailPage(selectedItem, _mediaService);

                // Suscribirse al evento de eliminación
                detailPage.MediaDeleted += async (s, itemId) =>
                {
                    await DeleteMediaItem(itemId);
                };

                await Navigation.PushAsync(detailPage);
            }
            else if (selectedItem.Type == "Video")
            {
                var playerPage = new VideoPlayerPage(selectedItem, _mediaService);

                // Suscribirse al evento de eliminación
                playerPage.MediaDeleted += async (s, itemId) =>
                {
                    await DeleteMediaItem(itemId);
                };

                await Navigation.PushAsync(playerPage);
            }

            // Desmarcar la selección
            MediaCollection.SelectedItem = null;
        }
    }

    private async Task DeleteMediaItem(string itemId)
    {
        var itemToRemove = _mediaItems.FirstOrDefault(item => item.Id == itemId);
        if (itemToRemove != null)
        {
            _mediaItems.Remove(itemToRemove);
            RefreshMediaCollection();

            // Guardar los cambios
            await _storageService.SaveMediaListAsync(_mediaItems);
        }
    }
}