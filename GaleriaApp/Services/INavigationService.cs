// Nuevo archivo: Services/INavigationService.cs
namespace GaleriaApp.Services
{
    public interface INavigationService
    {
        Task NavigateToAsync<T>(object parameter = null);
        Task NavigateBackAsync();
    }
}