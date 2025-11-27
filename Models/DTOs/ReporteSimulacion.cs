using System.Collections.Generic;
using System.Windows; // Necesario para la clase Point

namespace CajaExpressSim.Models.DTOs
{
    public class ReporteSimulacion
    {
        public int TotalClientesAtendidos { get; set; }
        public int CantidadClientesRechazados { get; set; }
        public int CantidadEstandar { get; set; }
        public int CantidadExpress { get; set; }
        public int CantidadCarro { get; set; }
        public double TiempoPromedioEnSistema { get; set; }
        public double TiempoPromedioEnCola { get; set; }
        public double LargoColaPromedio { get; set; }
        public double TiempoMaximoEspera { get; set; }
        public int LongitudMaximaCola { get; set; }
        public double Percentil50 { get; set; }
        public double Percentil90 { get; set; }
        public double Percentil95 { get; set; }
        public Dictionary<int, double> UtilizacionPorCaja { get; set; }
        public List<Point> GraficoEsperaPorHora { get; set; }

        public ReporteSimulacion()
        {
            UtilizacionPorCaja = new Dictionary<int, double>();
            GraficoEsperaPorHora = new List<Point>();
        }
    }
}