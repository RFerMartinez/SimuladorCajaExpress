using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CajaExpressSim.Models.Config;
using CajaExpressSim.Helpers;

namespace CajaExpressSim.ViewModels
{
    public class ConfiguracionViewModel : INotifyPropertyChanged
    {
        // Configuración de Horizonte ---
        private int _semanas;
        public int Semanas
        {
            get => _semanas;
            set { _semanas = value; OnPropertyChanged(); }
        }

        private int _diasPorSemana;
        public int DiasPorSemana
        {
            get => _diasPorSemana;
            set { _diasPorSemana = value; OnPropertyChanged(); }
        }

        // 1. Recursos
        private int _cantidadCajas;
        public int CantidadCajas
        {
            get => _cantidadCajas;
            set { _cantidadCajas = value; OnPropertyChanged(); }
        }

        private double _tiempoCobro;
        public double TiempoCobro
        {
            get => _tiempoCobro;
            set { _tiempoCobro = value; OnPropertyChanged(); }
        }

        private int _capacidadCola;
        public int CapacidadCola
        {
            get => _capacidadCola;
            set { _capacidadCola = value; OnPropertyChanged(); }
        }

        // 2. TASAS DE LLEGADA (Directo 80, 140, 100)
        private double _tasaLlegada1;
        public double TasaLlegada1
        {
            get => _tasaLlegada1;
            set { _tasaLlegada1 = value; OnPropertyChanged(); }
        }

        private double _tasaLlegada2;
        public double TasaLlegada2
        {
            get => _tasaLlegada2;
            set { _tasaLlegada2 = value; OnPropertyChanged(); }
        }

        private double _tasaLlegada3;
        public double TasaLlegada3
        {
            get => _tasaLlegada3;
            set { _tasaLlegada3 = value; OnPropertyChanged(); }
        }

        // 3. TIEMPOS DE SERVICIO
        private double _mediaEstandar;
        public double MediaEstandar
        {
            get => _mediaEstandar;
            set { _mediaEstandar = value; OnPropertyChanged(); }
        }
        private double _desvioEstandar;
        public double DesvioEstandar
        {
            get => _desvioEstandar;
            set { _desvioEstandar = value; OnPropertyChanged(); }
        }

        private double _mediaExpress;
        public double MediaExpress
        {
            get => _mediaExpress;
            set { _mediaExpress = value; OnPropertyChanged(); }
        }
        private double _desvioExpress;
        public double DesvioExpress
        {
            get => _desvioExpress;
            set { _desvioExpress = value; OnPropertyChanged(); }
        }

        private double _mediaCarro;
        public double MediaCarro
        {
            get => _mediaCarro;
            set { _mediaCarro = value; OnPropertyChanged(); }
        }
        private double _desvioCarro;
        public double DesvioCarro
        {
            get => _desvioCarro;
            set { _desvioCarro = value; OnPropertyChanged(); }
        }

        public ICommand GuardarCommand { get; private set; }

        public ConfiguracionViewModel()
        {
            CargarValores();
            GuardarCommand = new RelayCommand(GuardarCambios);
        }

        private void CargarValores()
        {
            Semanas = ParametrosGlobales.SemanasASimular;
            DiasPorSemana = ParametrosGlobales.DiasLaboralesPorSemana;

            CantidadCajas = ParametrosGlobales.CantidadCajas;
            TiempoCobro = ParametrosGlobales.TiempoCobroSegundos;

            CapacidadCola = ParametrosGlobales.CapacidadColaPorCaja;

            TasaLlegada1 = ParametrosGlobales.TasaLlegadaFranja1;
            TasaLlegada2 = ParametrosGlobales.TasaLlegadaFranja2;
            TasaLlegada3 = ParametrosGlobales.TasaLlegadaFranja3;

            MediaEstandar = ParametrosGlobales.MediaEstandar;
            DesvioEstandar = ParametrosGlobales.DesvioEstandar;

            MediaExpress = ParametrosGlobales.MediaExpress;
            DesvioExpress = ParametrosGlobales.DesvioExpress;

            MediaCarro = ParametrosGlobales.MediaCarro;
            DesvioCarro = ParametrosGlobales.DesvioCarro;
        }

        private void GuardarCambios(object parameter)
        {
            try
            {
                if (CantidadCajas <= 0) throw new Exception("Debe haber al menos 1 caja.");
                if (TasaLlegada1 <= 0 || TasaLlegada2 <= 0 || TasaLlegada3 <= 0)
                    throw new Exception("Las tasas de llegada deben ser positivas.");

                ParametrosGlobales.SemanasASimular = Semanas;
                ParametrosGlobales.DiasLaboralesPorSemana = DiasPorSemana;

                ParametrosGlobales.CantidadCajas = CantidadCajas;
                ParametrosGlobales.TiempoCobroSegundos = TiempoCobro;

                ParametrosGlobales.CapacidadColaPorCaja = CapacidadCola;

                ParametrosGlobales.TasaLlegadaFranja1 = TasaLlegada1;
                ParametrosGlobales.TasaLlegadaFranja2 = TasaLlegada2;
                ParametrosGlobales.TasaLlegadaFranja3 = TasaLlegada3;

                ParametrosGlobales.MediaEstandar = MediaEstandar;
                ParametrosGlobales.DesvioEstandar = DesvioEstandar;
                ParametrosGlobales.MediaExpress = MediaExpress;
                ParametrosGlobales.DesvioExpress = DesvioExpress;
                ParametrosGlobales.MediaCarro = MediaCarro;
                ParametrosGlobales.DesvioCarro = DesvioCarro;

                MessageBox.Show("¡Configuración guardada correctamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}