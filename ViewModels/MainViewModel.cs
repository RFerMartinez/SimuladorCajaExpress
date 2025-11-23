using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CajaExpressSim.Models.Core;
using CajaExpressSim.Services;
using CajaExpressSim.Helpers;

namespace CajaExpressSim.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // ==========================================
        // SUB-VIEWMODELS (Las "Pestañas" de tu app)
        // ==========================================
        public ConfiguracionViewModel ConfiguracionVM { get; set; }
        public ResultadosViewModel ResultadosVM { get; set; }

        // ==========================================
        // ESTADO DE LA VISTA
        // ==========================================

        // Controla qué vista se muestra actualmente (Config vs Resultados)
        private object _vistaActual;
        public object VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }

        // Para mostrar una barra de carga o deshabilitar botones
        private bool _estaSimulando;
        public bool EstaSimulando
        {
            get => _estaSimulando;
            set
            {
                _estaSimulando = value;
                OnPropertyChanged();
                // Forzamos actualización del comando para habilitar/deshabilitar botón
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _estadoSimulacion;
        public string EstadoSimulacion
        {
            get => _estadoSimulacion;
            set { _estadoSimulacion = value; OnPropertyChanged(); }
        }

        // ==========================================
        // EL MOTOR (El Corazón)
        // ==========================================
        private MotorSimulacion _motor;

        // ==========================================
        // COMANDOS
        // ==========================================
        public ICommand IniciarSimulacionCommand { get; private set; }
        public ICommand VolverConfiguracionCommand { get; private set; }

        // ==========================================
        // CONSTRUCTOR
        // ==========================================
        public MainViewModel()
        {
            // 1. Inicializar los hijos
            ConfiguracionVM = new ConfiguracionViewModel();
            ResultadosVM = new ResultadosViewModel();
            _motor = new MotorSimulacion();

            // 2. Configurar estado inicial
            VistaActual = ConfiguracionVM; // Empezamos mostrando la configuración
            EstadoSimulacion = "Listo para iniciar";
            EstaSimulando = false;

            // 3. Inicializar Comandos
            IniciarSimulacionCommand = new RelayCommand(EjecutarSimulacion, PuedeSimular);
            VolverConfiguracionCommand = new RelayCommand(VolverConfiguracion);
        }

        // ==========================================
        // LÓGICA DE NAVEGACIÓN Y EJECUCIÓN
        // ==========================================

        private bool PuedeSimular(object obj)
        {
            return !EstaSimulando; // Solo se puede pulsar si NO está corriendo
        }

        private async void EjecutarSimulacion(object obj)
        {
            EstaSimulando = true;
            EstadoSimulacion = "Inicializando Motor...";

            try
            {
                // A. Preparación (Esto ocurre en el hilo principal)
                _motor.Inicializar();

                // B. Ejecución Pesada (Esto ocurre en un hilo secundario para no congelar la UI)
                await Task.Run(() =>
                {
                    EstadoSimulacion = "Simulando eventos...";
                    _motor.Simular();
                });

                // C. Finalización y Cálculos (De vuelta al hilo principal)
                EstadoSimulacion = "Calculando estadísticas finales...";

                var reporteFinal = CalculadoraEstadisticas.GenerarReporte(
                    _motor.ClientesAtendidos,
                    _motor.Cajas,
                    _motor.GestorColas.MaximaLongitudRegistrada,
                    _motor.Reloj.TiempoActual
                );

                // D. Pasar datos a la vista de resultados
                ResultadosVM.CargarDatos(reporteFinal);

                // E. Cambiar de pantalla
                VistaActual = ResultadosVM;
                EstadoSimulacion = "Simulación Completada con Éxito.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error crítico durante la simulación: {ex.Message}", "Error Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
                EstadoSimulacion = "Error en la simulación.";
            }
            finally
            {
                EstaSimulando = false;
            }
        }

        private void VolverConfiguracion(object obj)
        {
            VistaActual = ConfiguracionVM;
            EstadoSimulacion = "Listo para nueva simulación";
        }

        // ==========================================
        // INotifyPropertyChanged
        // ==========================================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}