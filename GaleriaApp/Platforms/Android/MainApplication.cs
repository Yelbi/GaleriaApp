using Android.App;
using Android.Runtime;

namespace GaleriaApp
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        // Agregar estas líneas para el problema de MediaElement
        public override void OnCreate()
        {
            try
            {
                base.OnCreate();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en OnCreate: {ex}");
                throw;
            }
        }
    }
}