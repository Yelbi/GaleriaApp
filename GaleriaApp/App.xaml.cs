namespace GaleriaApp
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                // Intentar inicializar componentes
                InitializeComponent();
            }
            catch (System.IO.FileNotFoundException ex) when (ex.Message.Contains("WinRT.Runtime"))
            {
                // Error específico de WinRT - continuar sin los recursos problemáticos
                System.Diagnostics.Debug.WriteLine($"WinRT Runtime error: {ex.Message}");

                // Crear resources mínimos
                Resources = new ResourceDictionary();

                // Intentar cargar solo recursos esenciales
                try
                {
                    var colorsDict = new ResourceDictionary();
                    colorsDict.Source = new Uri("Resources/Styles/Colors.xaml", UriKind.Relative);
                    Resources.MergedDictionaries.Add(colorsDict);

                    var stylesDict = new ResourceDictionary();
                    stylesDict.Source = new Uri("Resources/Styles/Styles.xaml", UriKind.Relative);
                    Resources.MergedDictionaries.Add(stylesDict);
                }
                catch (Exception resourceEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al cargar recursos: {resourceEx.Message}");

                    // Crear recursos básicos en código
                    Resources.Add("Primary", Color.FromRgb(81, 43, 212));
                    Resources.Add("Gray100", Color.FromRgb(225, 225, 225));
                    Resources.Add("White", Colors.White);
                    Resources.Add("Black", Colors.Black);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general al inicializar App: {ex.Message}");

                // Inicialización mínima para que la app no falle
                Resources = new ResourceDictionary();

                // Agregar colores básicos
                Resources.Add("Primary", Color.FromRgb(81, 43, 212));
                Resources.Add("Gray100", Color.FromRgb(225, 225, 225));
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                return new Window(new AppShell());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando ventana: {ex.Message}");

                // Si AppShell falla, crear una página simple
                return new Window(new ContentPage
                {
                    Title = "GaleriaApp",
                    Content = new Label
                    {
                        Text = "Error al inicializar la aplicación. Por favor, reinicie.",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                });
            }
        }
    }
}