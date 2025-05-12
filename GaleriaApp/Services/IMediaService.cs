using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GaleriaApp.Models;

namespace GaleriaApp.Services
{
    public interface IMediaService
    {
        Task<List<MediaItem>> GetMediaFromDeviceAsync();
        Task<FileResult> PickPhotoAsync();
        Task<FileResult> PickVideoAsync();
    }
}
