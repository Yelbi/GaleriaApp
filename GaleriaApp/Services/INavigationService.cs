using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Nuevo archivo: Services/INavigationService.cs
namespace GaleriaApp.Services
{
    public interface INavigationService
    {
        Task NavigateToAsync<T>(object parameter = null);
        Task NavigateBackAsync();
    }
}