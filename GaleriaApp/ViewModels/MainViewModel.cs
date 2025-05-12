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

        public ObservableCollection<MediaItem> MediaItems { get; } = new ObservableCollection<MediaItem>();

        public ICommand SelectPhotoCommand { get; }
        public ICommand SelectVideoCommand { get; }
        public ICommand MediaSelectedCommand { get; }
        public ICommand DeleteMediaCommand { get; }

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

        private void ApplyFilter()
        {
            var filteredItems = _allMediaItems;

            // Filtrar por tipo
            if (!string.IsNullOrEmpty(SelectedMediaType) && SelectedMediaType != "All")
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
    }
}