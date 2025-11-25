using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CajaExpressSim.Models.Core;
using CajaExpressSim.Models.Config;
using CajaExpressSim.Models.Entidades;
using CajaExpressSim.Services;
using CajaExpressSim.Helpers;

namespace CajaExpressSim.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ConfiguracionViewModel ConfiguracionVM { get; set; }
        public ResultadosViewModel ResultadosVM { get; set; }

        private object _vistaActual;
        public object VistaActual
        {
            get => _vistaActual;
            set
            {
                _vistaActual = value;
                OnPropertyChanged();
                // Notificamos cambios en los estados visuales
                OnPropertyChanged(nameof(EsVistaResultados));
                OnPropertyChanged(nameof(EsVistaConfiguracion));
            }
        }

        // Propiedades para controlar visibilidad de botones
        public bool EsVistaResultados => VistaActual is ResultadosViewModel;
        public bool EsVistaConfiguracion => VistaActual is ConfiguracionViewModel; // NUEVO

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
                int diasTotales = ParametrosGlobales.SemanasASimular * ParametrosGlobales.DiasLaboralesPorSemana;
                List<Cliente> historialGlobal = new List<Cliente>();
                Dictionary<int, double> tiemposCajasGlobal = new Dictionary<int, double>();
                int maxColaGlobal = 0;
                int totalRechazadosGlobal = 0;

                await Task.Run(() =>
                {
                    for (int dia = 1; dia <= diasTotales; dia++)
                    {
                        EstadoSimulacion = $"Simulando Día {dia} de {diasTotales}...";
                        _motor.Inicializar();
                        _motor.Simular();

                        historialGlobal.AddRange(_motor.ClientesAtendidos);
                        totalRechazadosGlobal += _motor.ClientesRechazados;

                        if (_motor.GestorColas.MaximaLongitudRegistrada > maxColaGlobal)
                            maxColaGlobal = _motor.GestorColas.MaximaLongitudRegistrada;

                        foreach (var caja in _motor.Cajas)
                        {
                            if (!tiemposCajasGlobal.ContainsKey(caja.Id)) tiemposCajasGlobal[caja.Id] = 0;
                            tiemposCajasGlobal[caja.Id] += caja.TiempoTotalOcupada;
                        }
                    }
                });

                EstadoSimulacion = "Calculando estadísticas consolidadas...";
                double tiempoTotalSimulado = 50400.0 * diasTotales;

                var reporteFinal = CalculadoraEstadisticas.GenerarReporteConsolidado(
                    historialGlobal,
                    tiemposCajasGlobal,
                    maxColaGlobal,
                    tiempoTotalSimulado,
                    totalRechazadosGlobal
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