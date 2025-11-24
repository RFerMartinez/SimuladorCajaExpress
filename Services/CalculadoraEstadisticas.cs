using System;
using System.Collections.Generic;
using System.Linq;
using CajaExpressSim.Models.Entidades;
using CajaExpressSim.Models.DTOs;

namespace CajaExpressSim.Services
{
    public static class CalculadoraEstadisticas
    {
        public static ReporteSimulacion GenerarReporteConsolidado(
            List<Cliente> clientes,
            Dictionary<int, double> tiemposCajasAcumulados,
            int maxColaRegistrada,
            double tiempoTotalSimulacion)
        {
            var reporte = new ReporteSimulacion();
            reporte.TotalClientesAtendidos = clientes.Count;

            if (reporte.TotalClientesAtendidos == 0) return reporte;

            // 1. Extraer listas de tiempos para cálculos
            var tiemposEspera = clientes.Select(c => c.ObtenerTiempoEspera()).OrderBy(t => t).ToList();
            var tiemposSistema = clientes.Select(c => c.ObtenerTiempoEnSistema()).ToList();

            // 2. Promedios (W y Wq)
            reporte.TiempoPromedioEnSistema = tiemposSistema.Average();
            reporte.TiempoPromedioEnCola = tiemposEspera.Average();

            // 3. NUEVO: Calcular Largo Promedio de Cola (Lq)
            // Fórmula: Lq = (Suma Total de Tiempos de Espera) / Tiempo Total de Simulación
            // Esto es equivalente a la Ley de Little: Lq = Lambda * Wq
            double sumaTotalEspera = tiemposEspera.Sum();
            if (tiempoTotalSimulacion > 0)
            {
                reporte.LargoColaPromedio = sumaTotalEspera / tiempoTotalSimulacion;
            }

            // 4. Picos
            reporte.TiempoMaximoEspera = tiemposEspera.LastOrDefault(); // El último es el mayor porque ordenamos
            reporte.LongitudMaximaCola = maxColaRegistrada;

            // 5. NUEVO: Cálculo de Percentiles
            reporte.Percentil50 = CalcularPercentil(tiemposEspera, 0.50);
            reporte.Percentil90 = CalcularPercentil(tiemposEspera, 0.90);
            reporte.Percentil95 = CalcularPercentil(tiemposEspera, 0.95);

            // 6. Utilización
            foreach (var kvp in tiemposCajasAcumulados)
            {
                double utilizacion = 0;
                if (tiempoTotalSimulacion > 0)
                    utilizacion = (kvp.Value / tiempoTotalSimulacion) * 100;

                reporte.UtilizacionPorCaja.Add(kvp.Key, utilizacion);
            }

            return reporte;
        }

        // Método auxiliar para percentiles
        private static double CalcularPercentil(List<double> listaOrdenada, double percentil)
        {
            if (listaOrdenada.Count == 0) return 0;

            int indice = (int)Math.Ceiling(percentil * listaOrdenada.Count) - 1;
            if (indice < 0) indice = 0;
            if (indice >= listaOrdenada.Count) indice = listaOrdenada.Count - 1;

            return listaOrdenada[indice];
        }
    }
}