using System;
using System.Collections.ObjectModel; // Para listas dinámicas en la UI
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32; // Para abrir la ventana de "Guardar Archivo"
using CajaExpressSim.Models.DTOs;
using CajaExpressSim.Services;
using CajaExpressSim.Helpers;

namespace CajaExpressSim.ViewModels
{
    public class ResultadosViewModel : INotifyPropertyChanged
    {
        // Guardamos el reporte completo internamente
        private ReporteSimulacion _reporteActual;

        // ==========================================
        // PROPIEDADES VISUALES (Bindings)
        // ==========================================
        // Estas son las variables que verá tu pantalla XAML

        private int _totalClientes;
        public int TotalClientes
        {
            get => _totalClientes;
            set { _totalClientes = value; OnPropertyChanged(); }
        }

        private double _promedioSistema;
        public double PromedioSistema
        {
            get => _promedioSistema;
            set { _promedioSistema = value; OnPropertyChanged(); }
        }

        private double _promedioCola;
        public double PromedioCola
        {
            get => _promedioCola;
            set { _promedioCola = value; OnPropertyChanged(); }
        }

        private double _maxEspera;
        public double MaxEspera
        {
            get => _maxEspera;
            set { _maxEspera = value; OnPropertyChanged(); }
        }

        private int _maxCola;
        public int MaxCola
        {
            get => _maxCola;
            set { _maxCola = value; OnPropertyChanged(); }
        }

        // Lista especial para mostrar la tabla de Cajas en la UI
        public ObservableCollection<UtilizacionItem> ListaUtilizacion { get; set; }

        // ==========================================
        // COMANDOS (Botones)
        // ==========================================
        public ICommand ExportarPdfCommand { get; private set; }
        public ICommand ExportarTxtCommand { get; private set; }

        // ==========================================
        // CONSTRUCTOR
        // ==========================================
        public ResultadosViewModel()
        {
            ListaUtilizacion = new ObservableCollection<UtilizacionItem>();

            ExportarPdfCommand = new RelayCommand(ExportarPDF);
            ExportarTxtCommand = new RelayCommand(ExportarTXT);
        }

        // ==========================================
        // MÉTODOS DE LÓGICA
        // ==========================================

        // Este método lo llamaremos desde el MainViewModel cuando termine la simulación
        public void CargarDatos(ReporteSimulacion reporte)
        {
            _reporteActual = reporte;

            // 1. Asignar propiedades simples
            TotalClientes = reporte.TotalClientesAtendidos;
            PromedioSistema = reporte.TiempoPromedioEnSistema;
            PromedioCola = reporte.TiempoPromedioEnCola;
            MaxEspera = reporte.TiempoMaximoEspera;
            MaxCola = reporte.LongitudMaximaCola;

            // 2. Llenar la lista de utilización
            ListaUtilizacion.Clear();
            foreach (var kvp in reporte.UtilizacionPorCaja)
            {
                ListaUtilizacion.Add(new UtilizacionItem
                {
                    IdCaja = kvp.Key,
                    Porcentaje = kvp.Value
                });
            }
        }

        private void ExportarPDF(object obj)
        {
            if (_reporteActual == null) return;
            GuardarArchivo("Documento PDF (*.pdf)|*.pdf", ruta =>
            {
                ExportadorPDF.GuardarReporte(ruta, _reporteActual);
            });
        }

        private void ExportarTXT(object obj)
        {
            if (_reporteActual == null) return;
            GuardarArchivo("Texto Plano (*.txt)|*.txt", ruta =>
            {
                ExportadorTXT.GuardarReporte(ruta, _reporteActual);
            });
        }

        // Helper para abrir el diálogo de guardar
        private void GuardarArchivo(string filtro, Action<string> accionGuardar)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = filtro;
            sfd.FileName = $"Reporte_CajaExpress_{DateTime.Now:yyyyMMdd_HHmm}";

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    accionGuardar(sfd.FileName);
                    MessageBox.Show("Reporte exportado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Clase auxiliar pequeña para mostrar la lista en la tabla visual
    public class UtilizacionItem
    {
        public int IdCaja { get; set; }
        public double Porcentaje { get; set; }
    }
}