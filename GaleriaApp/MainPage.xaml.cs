using GaleriaApp.Models;
using GaleriaApp.Services;
using GaleriaApp.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace GaleriaApp
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;
        private readonly IMediaService _mediaService;
        private readonly IStorageService _storageService;
        private bool _isSearchActive = false;

        public MainPage(IMediaService mediaService, IStorageService storageService)
        {
            // Inicializar componentes first
            InitializeComponent();

            _mediaService = mediaService;
            _storageService = storageService;

            // Inicializar el ViewModel
            _viewModel = new MainViewModel(mediaService, storageService);
            BindingContext = _viewModel;

            // Cargar medios guardados al iniciar
            LoadSavedMediaAsync();
        }

        private async Task LoadSavedMediaAsync()
        {
            try
            {
                var mediaItems = await _storageService.LoadMediaListAsync();
                _viewModel.LoadMediaItems(mediaItems);
                MediaCollection.ItemsSource = _viewModel.MediaItems;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading media: {ex.Message}");
            }
        }

        private async void OnSelectPhotoClicked(object sender, EventArgs e)
        {
            await ExecuteSelectPhotoCommand();
        }

        private async Task ExecuteSelectPhotoCommand()
        {
            var photo = await _mediaService.PickPhotoAsync();
            if (photo is not null)
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

                _viewModel.AddMediaItem(newItem);
                RefreshMediaCollection();

                // Guardar cambios
                await _storageService.SaveMediaListAsync(_viewModel.MediaItems.ToList());
            }
        }

        private async void OnSelectVideoClicked(object sender, EventArgs e)
        {
            await ExecuteSelectVideoCommand();
        }

        private async Task ExecuteSelectVideoCommand()
        {
            var video = await _mediaService.PickVideoAsync();
            if (video is not null)
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

                _viewModel.AddMediaItem(newItem);
                RefreshMediaCollection();

                // Guardar cambios
                await _storageService.SaveMediaListAsync(_viewModel.MediaItems.ToList());
            }
        }

        private void RefreshMediaCollection()
        {
            MediaCollection.ItemsSource = null;
            MediaCollection.ItemsSource = _viewModel.MediaItems;
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
                        await DeleteMediaItemAsync(itemId);
                    };

                    await Navigation.PushAsync(detailPage);
                }
                else if (selectedItem.Type == "Video")
                {
                    var playerPage = new VideoPlayerPage(selectedItem, _mediaService);

                    // Suscribirse al evento de eliminación
                    playerPage.MediaDeleted += async (s, itemId) =>
                    {
                        await DeleteMediaItemAsync(itemId);
                    };

                    await Navigation.PushAsync(playerPage);
                }

                // Desmarcar la selección
                MediaCollection.SelectedItem = null;
            }
        }

        private async Task DeleteMediaItemAsync(string itemId)
        {
            _viewModel.RemoveMediaItem(itemId);
            RefreshMediaCollection();

            // Guardar los cambios
            await _storageService.SaveMediaListAsync(_viewModel.MediaItems.ToList());
        }

        // Métodos para funciones de UI mejoradas
        private void OnSearchToggleClicked(object sender, EventArgs e)
        {
            _isSearchActive = !_isSearchActive;
            _viewModel.IsSearchActive = _isSearchActive;
        }

        private void OnGridViewClicked(object sender, EventArgs e)
        {
            _viewModel.GridSpan = 2;
            _viewModel.IsGridView = true;
            _viewModel.IsListView = false;
        }

        private void OnListViewClicked(object sender, EventArgs e)
        {
            _viewModel.GridSpan = 1;
            _viewModel.IsGridView = false;
            _viewModel.IsListView = true;
        }

        private async void OnMoreOptionsClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet(
                "Opciones adicionales",
                "Cancelar",
                null,
                "Tomar foto",
                "Grabar video",
                "Crear nuevo álbum",
                "Ordenar por fecha",
                "Ordenar por nombre",
                "Preferencias",
                "Acerca de");

            switch (action)
            {
                case "Tomar foto":
                    await ExecuteTakePhotoCommand();
                    break;
                case "Grabar video":
                    await ExecuteCaptureVideoCommand();
                    break;
                case "Crear nuevo álbum":
                    await HandleCreateAlbum();
                    break;
                case "Ordenar por fecha":
                    _viewModel.SortByDate();
                    break;
                case "Ordenar por nombre":
                    _viewModel.SortByName();
                    break;
                case "Preferencias":
                    await HandlePreferences();
                    break;
                case "Acerca de":
                    await DisplayAlert("Acerca de", "GaleriaApp v1.0\nUna aplicación para gestionar tus fotos y videos.", "Cerrar");
                    break;
            }
        }

        private async Task ExecuteTakePhotoCommand()
        {
            var photo = await _mediaService.TakePhotoAsync();
            if (photo is not null)
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

                _viewModel.AddMediaItem(newItem);
                RefreshMediaCollection();

                // Guardar cambios
                await _storageService.SaveMediaListAsync(_viewModel.MediaItems.ToList());
            }
        }

        private async Task ExecuteCaptureVideoCommand()
        {
            var video = await _mediaService.CaptureVideoAsync();
            if (video is not null)
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

                _viewModel.AddMediaItem(newItem);
                RefreshMediaCollection();

                // Guardar cambios
                await _storageService.SaveMediaListAsync(_viewModel.MediaItems.ToList());
            }
        }

        private async Task HandleCreateAlbum()
        {
            string albumName = await DisplayPromptAsync(
                "Nuevo Álbum",
                "Introduce un nombre para el álbum:",
                accept: "Crear",
                cancel: "Cancelar");

            if (!string.IsNullOrWhiteSpace(albumName))
            {
                // Aquí iría la lógica para crear el álbum
                await DisplayAlert("Álbum Creado", $"El álbum '{albumName}' ha sido creado.", "OK");
            }
        }

        private async Task HandlePreferences()
        {
            string action = await DisplayActionSheet(
                "Preferencias",
                "Cancelar",
                null,
                "Tema claro",
                "Tema oscuro",
                "Usar tema del sistema");

            switch (action)
            {
                case "Tema claro":
                    Application.Current.UserAppTheme = AppTheme.Light;
                    break;
                case "Tema oscuro":
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    break;
                case "Usar tema del sistema":
                    Application.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
            }
        }
    }
}