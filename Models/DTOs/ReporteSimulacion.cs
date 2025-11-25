using System.Collections.Generic;

namespace CajaExpressSim.Models.DTOs
{
    public class ReporteSimulacion
    {
        // Métricas Generales
        public int TotalClientesAtendidos { get; set; }

        public int CantidadClientesRechazados { get; set; }

        public int CantidadEstandar { get; set; }
        public int CantidadExpress { get; set; }
        public int CantidadCarro { get; set; }

        public double TiempoPromedioEnSistema { get; set; } // W
        public double TiempoPromedioEnCola { get; set; }    // Wq

        // NUEVO: Largo de Cola Promedio (Lq)
        public double LargoColaPromedio { get; set; }       // Lq

        // Métricas de Calidad (Picos y Percentiles)
        public double TiempoMaximoEspera { get; set; }
        public int LongitudMaximaCola { get; set; }

        // NUEVO: Percentiles
        public double Percentil50 { get; set; } // Mediana
        public double Percentil90 { get; set; }
        public double Percentil95 { get; set; }

        // Utilización
        public Dictionary<int, double> UtilizacionPorCaja { get; set; }

        public ReporteSimulacion()
        {
            UtilizacionPorCaja = new Dictionary<int, double>();
        }
    }
}