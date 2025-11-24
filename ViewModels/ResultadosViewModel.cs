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

        // KPIs Superiores
        private double _promedioSistema; public double PromedioSistema { get => _promedioSistema; set { _promedioSistema = value; OnPropertyChanged(); } }
        private double _promedioCola; public double PromedioCola { get => _promedioCola; set { _promedioCola = value; OnPropertyChanged(); } }

        // NUEVO: Lq
        private double _largoColaPromedio;
        public double LargoColaPromedio { get => _largoColaPromedio; set { _largoColaPromedio = value; OnPropertyChanged(); } }

        private int _maxCola; public int MaxCola { get => _maxCola; set { _maxCola = value; OnPropertyChanged(); } }

        // Métricas de Detalle y Críticas
        private int _totalClientes; public int TotalClientes { get => _totalClientes; set { _totalClientes = value; OnPropertyChanged(); } }
        private double _maxEspera; public double MaxEspera { get => _maxEspera; set { _maxEspera = value; OnPropertyChanged(); } }

        // NUEVO: Percentiles
        private double _p50; public double P50 { get => _p50; set { _p50 = value; OnPropertyChanged(); } }
        private double _p90; public double P90 { get => _p90; set { _p90 = value; OnPropertyChanged(); } }
        private double _p95; public double P95 { get => _p95; set { _p95 = value; OnPropertyChanged(); } }

        public ObservableCollection<UtilizacionItem> ListaUtilizacion { get; set; }
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

            // Asignaciones
            PromedioSistema = reporte.TiempoPromedioEnSistema;
            PromedioCola = reporte.TiempoPromedioEnCola;
            LargoColaPromedio = reporte.LargoColaPromedio; // NUEVO
            MaxCola = reporte.LongitudMaximaCola;
            TotalClientes = reporte.TotalClientesAtendidos;
            MaxEspera = reporte.TiempoMaximoEspera;

            // Percentiles NUEVOS
            P50 = reporte.Percentil50;
            P90 = reporte.Percentil90;
            P95 = reporte.Percentil95;

            // Lista
            ListaUtilizacion.Clear();
            foreach (var kvp in reporte.UtilizacionPorCaja)
            {
                ListaUtilizacion.Add(new UtilizacionItem { IdCaja = kvp.Key, Porcentaje = kvp.Value });
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