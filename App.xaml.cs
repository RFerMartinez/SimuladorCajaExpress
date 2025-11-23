using System.Windows;
// Asegúrate de que estas líneas coincidan con tus carpetas reales.
// Si te marca error en "CajaExpressSim", cámbialo por "SIMULADOR" en tus archivos
// o agrega "using CajaExpressSim.Views;" si mantuviste mis namespaces.
using CajaExpressSim.Views;
using CajaExpressSim.ViewModels;

namespace SIMULADOR
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Crear la ventana visual (View)
            MainWindow window = new MainWindow();

            // 2. Crear la lógica (ViewModel)
            MainViewModel viewModel = new MainViewModel();

            // 3. CONECTARLOS (Esto es lo que hace que aparezcan los datos)
            window.DataContext = viewModel;

            // 4. Mostrar la ventana
            window.Show();
        }
    }
}