using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaleriaApp.Services
{
    public interface IThemeService
    {
        void SetTheme(AppTheme theme);
        AppTheme GetCurrentTheme();
        AppTheme GetSystemTheme();
        bool IsUsingSystemTheme();
        void ToggleTheme();
    }

    public class ThemeService : IThemeService
    {
        private const string ThemePreferenceKey = "app_theme";
        private const string UseSystemThemeKey = "use_system_theme";

        public void SetTheme(AppTheme theme)
        {
            Application.Current.UserAppTheme = theme;
            Preferences.Set(ThemePreferenceKey, theme.ToString());
            Preferences.Set(UseSystemThemeKey, false);
        }

        public AppTheme GetCurrentTheme()
        {
            return Application.Current.RequestedTheme;
        }

        public AppTheme GetSystemTheme()
        {
            return Application.Current.PlatformAppTheme;
        }

        public bool IsUsingSystemTheme()
        {
            return Preferences.Get(UseSystemThemeKey, true);
        }

        public void ToggleTheme()
        {
            var currentTheme = Application.Current.UserAppTheme;
            var newTheme = currentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
            SetTheme(newTheme);
        }

        public void UseSystemTheme()
        {
            Application.Current.UserAppTheme = AppTheme.Unspecified;
            Preferences.Set(UseSystemThemeKey, true);
        }
    }
}