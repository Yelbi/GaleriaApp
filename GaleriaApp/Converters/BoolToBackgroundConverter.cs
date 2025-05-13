using System.Globalization;

namespace GaleriaApp.Converters
{
    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                // Usar el color primario de la aplicación cuando está activo
                if (Application.Current.Resources.TryGetValue("Primary", out var primaryColor))
                {
                    return primaryColor;
                }
                return Color.FromRgb(81, 43, 212); // Fallback al color primario
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}