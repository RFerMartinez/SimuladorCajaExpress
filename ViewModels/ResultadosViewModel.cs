using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using CajaExpressSim.Models.DTOs;
using CajaExpressSim.Services;
using CajaExpressSim.Helpers;

namespace CajaExpressSim.ViewModels
{
    public class ResultadosViewModel : INotifyPropertyChanged
    {
        private ReporteSimulacion _reporteActual;

        // KPIs Superiores
        private double _promedioSistema; public double PromedioSistema { get => _promedioSistema; set { _promedioSistema = value; OnPropertyChanged(); } }
        private double _promedioCola; public double PromedioCola { get => _promedioCola; set { _promedioCola = value; OnPropertyChanged(); } }
        private double _largoColaPromedio; public double LargoColaPromedio { get => _largoColaPromedio; set { _largoColaPromedio = value; OnPropertyChanged(); } }
        private int _maxCola; public int MaxCola { get => _maxCola; set { _maxCola = value; OnPropertyChanged(); } }

        // Totales y Rechazos
        private int _totalClientes; public int TotalClientes { get => _totalClientes; set { _totalClientes = value; OnPropertyChanged(); } }
        private int _rechazados; public int Rechazados { get => _rechazados; set { _rechazados = value; OnPropertyChanged(); } }

        // NUEVO: Desglose
        private int _cantEstandar; public int CantEstandar { get => _cantEstandar; set { _cantEstandar = value; OnPropertyChanged(); } }
        private int _cantExpress; public int CantExpress { get => _cantExpress; set { _cantExpress = value; OnPropertyChanged(); } }
        private int _cantCarro; public int CantCarro { get => _cantCarro; set { _cantCarro = value; OnPropertyChanged(); } }

        // Calidad
        private double _maxEspera; public double MaxEspera { get => _maxEspera; set { _maxEspera = value; OnPropertyChanged(); } }
        private double _p50; public double P50 { get => _p50; set { _p50 = value; OnPropertyChanged(); } }
        private double _p90; public double P90 { get => _p90; set { _p90 = value; OnPropertyChanged(); } }
        private double _p95; public double P95 { get => _p95; set { _p95 = value; OnPropertyChanged(); } }

        public ObservableCollection<UtilizacionItem> ListaUtilizacion { get; set; }
        public ICommand ExportarPdfCommand { get; private set; }
        public ICommand ExportarTxtCommand { get; private set; }

        public ResultadosViewModel()
        {
            ListaUtilizacion = new ObservableCollection<UtilizacionItem>();
            ExportarPdfCommand = new RelayCommand(ExportarPDF);
            ExportarTxtCommand = new RelayCommand(ExportarTXT);
        }

        public void CargarDatos(ReporteSimulacion reporte)
        {
            _reporteActual = reporte;

            PromedioSistema = reporte.TiempoPromedioEnSistema;
            PromedioCola = reporte.TiempoPromedioEnCola;
            LargoColaPromedio = reporte.LargoColaPromedio;
            MaxCola = reporte.LongitudMaximaCola;

            TotalClientes = reporte.TotalClientesAtendidos;
            Rechazados = reporte.CantidadClientesRechazados;

            // NUEVO
            CantEstandar = reporte.CantidadEstandar;
            CantExpress = reporte.CantidadExpress;
            CantCarro = reporte.CantidadCarro;

            MaxEspera = reporte.TiempoMaximoEspera;
            P50 = reporte.Percentil50;
            P90 = reporte.Percentil90;
            P95 = reporte.Percentil95;

            ListaUtilizacion.Clear();
            foreach (var kvp in reporte.UtilizacionPorCaja)
            {
                ListaUtilizacion.Add(new UtilizacionItem { IdCaja = kvp.Key, Porcentaje = kvp.Value });
            }
        }

        private void ExportarPDF(object obj) { if (_reporteActual != null) GuardarArchivo("PDF (*.pdf)|*.pdf", r => ExportadorPDF.GuardarReporte(r, _reporteActual)); }
        private void ExportarTXT(object obj) { if (_reporteActual != null) GuardarArchivo("TXT (*.txt)|*.txt", r => ExportadorTXT.GuardarReporte(r, _reporteActual)); }

        private void GuardarArchivo(string filtro, Action<string> accionGuardar)
        {
            SaveFileDialog sfd = new SaveFileDialog(); sfd.Filter = filtro; sfd.FileName = $"Reporte_{DateTime.Now:yyyyMMdd_HHmm}";
            if (sfd.ShowDialog() == true) { try { accionGuardar(sfd.FileName); MessageBox.Show("Exportado correctamente."); } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class UtilizacionItem { public int IdCaja { get; set; } public double Porcentaje { get; set; } }
}