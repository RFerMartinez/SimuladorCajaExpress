using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CajaExpressSim.Models.Core;
using CajaExpressSim.Models.Config; // Importante
using CajaExpressSim.Models.Entidades; // Importante
using CajaExpressSim.Services;
using CajaExpressSim.Helpers;

namespace CajaExpressSim.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ConfiguracionViewModel ConfiguracionVM { get; set; }
        public ResultadosViewModel ResultadosVM { get; set; }

        private object _vistaActual;
        public object VistaActual { get => _vistaActual; set { _vistaActual = value; OnPropertyChanged(); } }

        private bool _estaSimulando;
        public bool EstaSimulando
        {
            get => _estaSimulando;
            set { _estaSimulando = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        private string _estadoSimulacion;
        public string EstadoSimulacion { get => _estadoSimulacion; set { _estadoSimulacion = value; OnPropertyChanged(); } }

        private MotorSimulacion _motor;

        public ICommand IniciarSimulacionCommand { get; private set; }
        public ICommand VolverConfiguracionCommand { get; private set; }

        public MainViewModel()
        {
            ConfiguracionVM = new ConfiguracionViewModel();
            ResultadosVM = new ResultadosViewModel();
            _motor = new MotorSimulacion();

            VistaActual = ConfiguracionVM;
            EstadoSimulacion = "Listo para iniciar";
            EstaSimulando = false;

            IniciarSimulacionCommand = new RelayCommand(EjecutarSimulacion, PuedeSimular);
            VolverConfiguracionCommand = new RelayCommand(VolverConfiguracion);
        }

        private bool PuedeSimular(object obj) => !EstaSimulando;

        private async void EjecutarSimulacion(object obj)
        {
            EstaSimulando = true;
            EstadoSimulacion = "Preparando simulación...";

            try
            {
                // 1. Calcular días totales
                int diasTotales = ParametrosGlobales.SemanasASimular * ParametrosGlobales.DiasLaboralesPorSemana;

                // Acumuladores GLOBALES para todo el periodo
                List<Cliente> historialGlobal = new List<Cliente>();
                Dictionary<int, double> tiemposCajasGlobal = new Dictionary<int, double>();
                int maxColaGlobal = 0;

                await Task.Run(() =>
                {
                    // BUCLE DE DÍAS
                    for (int dia = 1; dia <= diasTotales; dia++)
                    {
                        // Actualizar estado en UI (ej: "Simulando Día 3 de 24...")
                        // Nota: Como estamos en otro hilo, usamos Dispatcher si fuera WPF puro, 
                        // pero con el binding de string simple suele funcionar si la propiedad notifica.
                        EstadoSimulacion = $"Simulando Día {dia} de {diasTotales}...";

                        // A. Inicializar Motor (Resetea reloj, colas y cajas para un nuevo día)
                        _motor.Inicializar();

                        // B. Correr el día
                        _motor.Simular();

                        // C. ACUMULAR RESULTADOS DE ESTE DÍA
                        historialGlobal.AddRange(_motor.ClientesAtendidos);

                        if (_motor.GestorColas.MaximaLongitudRegistrada > maxColaGlobal)
                        {
                            maxColaGlobal = _motor.GestorColas.MaximaLongitudRegistrada;
                        }

                        // Acumular tiempo de uso de cajas
                        foreach (var caja in _motor.Cajas)
                        {
                            if (!tiemposCajasGlobal.ContainsKey(caja.Id))
                                tiemposCajasGlobal[caja.Id] = 0;

                            tiemposCajasGlobal[caja.Id] += caja.TiempoTotalOcupada;
                        }
                    }
                });

                EstadoSimulacion = "Calculando estadísticas consolidadas...";

                // Calcular tiempo total simulado (Segundos por día * días totales)
                // 14 horas operativas = 50400 segundos por día
                double tiempoTotalSimulado = 50400.0 * diasTotales;

                // Llamamos a la calculadora con los datos consolidados
                // NOTA: Tuvimos que sobrecargar GenerarReporte para aceptar el diccionario de cajas
                var reporteFinal = CalculadoraEstadisticas.GenerarReporteConsolidado(
                    historialGlobal,
                    tiemposCajasGlobal,
                    maxColaGlobal,
                    tiempoTotalSimulado
                );

                ResultadosVM.CargarDatos(reporteFinal);
                VistaActual = ResultadosVM;
                EstadoSimulacion = $"Simulación finalizada ({diasTotales} días).";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}