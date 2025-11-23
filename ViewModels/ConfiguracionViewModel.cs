using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CajaExpressSim.Models.Config;
// Asegúrate de tener la clase RelayCommand (te la paso más abajo)
using CajaExpressSim.Helpers;

namespace CajaExpressSim.ViewModels
{
    public class ConfiguracionViewModel : INotifyPropertyChanged
    {
        // ==========================================
        // PROPIEDADES ENLAZADAS A LA VISTA (BINDING)
        // ==========================================

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

        // 2. Llegadas (Medias en segundos)
        private double _mediaLlegada1;
        public double MediaLlegada1
        {
            get => _mediaLlegada1;
            set { _mediaLlegada1 = value; OnPropertyChanged(); }
        }

        private double _mediaLlegada2;
        public double MediaLlegada2
        {
            get => _mediaLlegada2;
            set { _mediaLlegada2 = value; OnPropertyChanged(); }
        }

        private double _mediaLlegada3;
        public double MediaLlegada3
        {
            get => _mediaLlegada3;
            set { _mediaLlegada3 = value; OnPropertyChanged(); }
        }

        // 3. Tiempos de Servicio (Cliente Estándar)
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

        // (Podrías agregar aquí las propiedades para Express y CarroCompleto igual que arriba)
        // Por brevedad, asumo que completarás las demás siguiendo el patrón.

        // ==========================================
        // COMANDOS (Botones)
        // ==========================================
        public ICommand GuardarCommand { get; private set; }

        // ==========================================
        // CONSTRUCTOR
        // ==========================================
        public ConfiguracionViewModel()
        {
            // 1. Cargar valores actuales de la configuración global
            CargarValores();

            // 2. Inicializar el comando del botón
            GuardarCommand = new RelayCommand(GuardarCambios);
        }

        private void CargarValores()
        {
            CantidadCajas = ParametrosGlobales.CantidadCajas;
            TiempoCobro = ParametrosGlobales.TiempoCobroSegundos;

            MediaLlegada1 = ParametrosGlobales.MediaLlegadaFranja1;
            MediaLlegada2 = ParametrosGlobales.MediaLlegadaFranja2;
            MediaLlegada3 = ParametrosGlobales.MediaLlegadaFranja3;

            MediaEstandar = ParametrosGlobales.MediaEstandar;
            DesvioEstandar = ParametrosGlobales.DesvioEstandar;
            // ... cargar el resto ...
        }

        // ==========================================
        // LÓGICA DEL BOTÓN GUARDAR
        // ==========================================
        private void GuardarCambios(object parameter)
        {
            try
            {
                // Validación básica
                if (CantidadCajas <= 0) throw new Exception("Debe haber al menos 1 caja.");
                if (MediaLlegada1 <= 0) throw new Exception("Los tiempos deben ser positivos.");

                // Transferir valores de la Pantalla a la clase Estática
                ParametrosGlobales.CantidadCajas = CantidadCajas;
                ParametrosGlobales.TiempoCobroSegundos = TiempoCobro;

                ParametrosGlobales.MediaLlegadaFranja1 = MediaLlegada1;
                ParametrosGlobales.MediaLlegadaFranja2 = MediaLlegada2;
                ParametrosGlobales.MediaLlegadaFranja3 = MediaLlegada3;

                ParametrosGlobales.MediaEstandar = MediaEstandar;
                ParametrosGlobales.DesvioEstandar = DesvioEstandar;

                MessageBox.Show("¡Configuración guardada correctamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==========================================
        // IMPLEMENTACIÓN INotifyPropertyChanged
        // ==========================================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}