// Nuevo archivo: ViewModels/MainViewModel.cs
using GaleriaApp.Models;
using GaleriaApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GaleriaApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IMediaService _mediaService;
        private readonly IStorageService _storageService;
        private List<MediaItem> _allMediaItems = new List<MediaItem>();

        public ObservableCollection<MediaItem> MediaItems { get; } = new ObservableCollection<MediaItem>();

        public ICommand SelectPhotoCommand { get; }
        public ICommand SelectVideoCommand { get; }
        public ICommand MediaSelectedCommand { get; }
        public ICommand DeleteMediaCommand { get; }

        // Propiedades para búsqueda
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                    ApplyFilter();
            }
        }

        private string _selectedMediaType;
        public string SelectedMediaType
        {
            get => _selectedMediaType;
            set
            {
                if (SetProperty(ref _selectedMediaType, value))
                    ApplyFilter();
            }
        }

        public MainViewModel(IMediaService mediaService, IStorageService storageService)
        {
            _mediaService = mediaService;
            _storageService = storageService;

            SelectPhotoCommand = new Command(async () => await SelectPhotoAsync());
            SelectVideoCommand = new Command(async () => await SelectVideoAsync());
            MediaSelectedCommand = new Command<MediaItem>(async (item) => await OnMediaSelected(item));
            DeleteMediaCommand = new Command<string>(async (id) => await DeleteMediaItemAsync(id));

            // Cargar los medios al inicio
            LoadSavedMediaAsync();
        }

        private async Task LoadSavedMediaAsync()
        {
            IsBusy = true;

            try
            {
                _allMediaItems = await _storageService.LoadMediaListAsync();

                MediaItems.Clear();
                foreach (var item in _allMediaItems)
                {
                    MediaItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error cargando medios: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SelectPhotoAsync()
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

                _allMediaItems.Add(newItem);
                MediaItems.Add(newItem);

                // Guardar cambios
                await _storageService.SaveMediaListAsync(_allMediaItems);
            }
        }

        private async Task SelectVideoAsync()
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

                _allMediaItems.Add(newItem);
                MediaItems.Add(newItem);

                // Guardar cambios
                await _storageService.SaveMediaListAsync(_allMediaItems);
            }
        }

        private async Task OnMediaSelected(MediaItem item)
        {
            // Implementación de navegación
            if (item == null) return;

            // Lógica de navegación aquí
        }

        private async Task DeleteMediaItemAsync(string itemId)
        {
            var itemToRemove = _allMediaItems.FirstOrDefault(item => item.Id == itemId);
            if (itemToRemove != null)
            {
                _allMediaItems.Remove(itemToRemove);

                // Actualizar colección observable
                var itemToRemoveFromObservable = MediaItems.FirstOrDefault(item => item.Id == itemId);
                if (itemToRemoveFromObservable != null)
                {
                    MediaItems.Remove(itemToRemoveFromObservable);
                }

                // Guardar los cambios
                await _storageService.SaveMediaListAsync(_allMediaItems);
            }
        }

        private void ApplyFilter()
        {
            var filteredItems = _allMediaItems;

            // Filtrar por tipo
            if (!string.IsNullOrEmpty(SelectedMediaType) && SelectedMediaType != "Todos")
            {
                filteredItems = filteredItems.Where(m => m.Type == SelectedMediaType).ToList();
            }

            // Filtrar por búsqueda
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                filteredItems = filteredItems.Where(m =>
                    m.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Actualizar colección observable
            MediaItems.Clear();
            foreach (var item in filteredItems)
            {
                MediaItems.Add(item);
            }
        }
        // Propiedades para la UI
        private bool _isSearchActive;
        public bool IsSearchActive
        {
            get => _isSearchActive;
            set => SetProperty(ref _isSearchActive, value);
        }

        private bool _isGridView = true;
        public bool IsGridView
        {
            get => _isGridView;
            set => SetProperty(ref _isGridView, value);
        }

        private bool _isListView;
        public bool IsListView
        {
            get => _isListView;
            set => SetProperty(ref _isListView, value);
        }

        private int _gridSpan = 2;
        public int GridSpan
        {
            get => _gridSpan;
            set => SetProperty(ref _gridSpan, value);
        }

        // Métodos para gestionar la lista
        public void LoadMediaItems(List<MediaItem> items)
        {
            _allMediaItems = items;
            MediaItems.Clear();
            foreach (var item in items)
            {
                MediaItems.Add(item);
            }
        }

        public void AddMediaItem(MediaItem item)
        {
            _allMediaItems.Add(item);
            MediaItems.Add(item);
        }

        public void RemoveMediaItem(string id)
        {
            var itemToRemove = _allMediaItems.FirstOrDefault(item => item.Id == id);
            if (itemToRemove != null)
            {
                _allMediaItems.Remove(itemToRemove);

                var itemToRemoveFromCollection = MediaItems.FirstOrDefault(item => item.Id == id);
                if (itemToRemoveFromCollection != null)
                {
                    MediaItems.Remove(itemToRemoveFromCollection);
                }
            }
        }

        public void SortByDate()
        {
            var sorted = _allMediaItems.OrderByDescending(i => i.DateCreated).ToList();
            _allMediaItems = sorted;

            MediaItems.Clear();
            foreach (var item in sorted)
            {
                MediaItems.Add(item);
            }
        }

        public void SortByName()
        {
            var sorted = _allMediaItems.OrderBy(i => i.Title).ToList();
            _allMediaItems = sorted;

            MediaItems.Clear();
            foreach (var item in sorted)
            {
                MediaItems.Add(item);
            }
        }
    }
}