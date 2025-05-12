using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar componentes: {ex.Message}");

                // Inicialización alternativa de recursos
                Resources = new ResourceDictionary();

                try
                {
                    Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("Resources/Styles/Colors.xaml", UriKind.Relative) });
                    Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("Resources/Styles/Styles.xaml", UriKind.Relative) });
                }
                catch (Exception resourceEx)
                {
                    Console.WriteLine($"Error al cargar recursos: {resourceEx.Message}");
                    // Continuar con recursos mínimos en caso de error
                }
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}