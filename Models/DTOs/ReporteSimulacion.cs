using System.Collections.Generic;

namespace CajaExpressSim.Models.DTOs
{
    // DTO: Data Transfer Object (Objeto simple para transportar datos)
    public class ReporteSimulacion
    {
        // ==========================================
        // MÉTRICAS GENERALES
        // ==========================================
        public int TotalClientesAtendidos { get; set; }

        // W: Tiempo desde que llega hasta que sale
        public double TiempoPromedioEnSistema { get; set; }

        // Wq: Tiempo desde que llega hasta que lo atienden
        public double TiempoPromedioEnCola { get; set; }

        // ==========================================
        // INDICADORES DE CALIDAD DE SERVICIO
        // ==========================================
        // El peor tiempo que alguien esperó
        public double TiempoMaximoEspera { get; set; }

        // La fila más larga que se formó
        public int LongitudMaximaCola { get; set; }

        // ==========================================
        // UTILIZACIÓN POR RECURSO
        // ==========================================
        // Diccionario: ID Caja (int) -> Porcentaje Ocupación (double)
        public Dictionary<int, double> UtilizacionPorCaja { get; set; }

        public ReporteSimulacion()
        {
            // Inicializamos el diccionario para evitar errores de nulos
            UtilizacionPorCaja = new Dictionary<int, double>();
        }
    }
}