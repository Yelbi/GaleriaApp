namespace GaleriaApp.Models
{
    public class MediaItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Type { get; set; } // "Image" o "Video"
        public DateTime DateCreated { get; set; }
    }
}