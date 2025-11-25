using CajaExpressSim.Models.DTOs;
using CajaExpressSim.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CajaExpressSim.Services
{
    public static class CalculadoraEstadisticas
    {
        public static ReporteSimulacion GenerarReporteConsolidado(
            List<Cliente> clientes,
            Dictionary<int, double> tiemposCajasAcumulados,
            int maxColaRegistrada,
            double tiempoTotalSimulacion,
            int totalRechazados) // NUEVO PARAMETRO
        {
            var reporte = new ReporteSimulacion();
            reporte.TotalClientesAtendidos = clientes.Count;

            // Asignar Rechazados
            reporte.CantidadClientesRechazados = totalRechazados;

            // --- NUEVO: Contar por tipo ---
            reporte.CantidadEstandar = clientes.Count(c => c.Tipo == TipoCliente.Estandar);
            reporte.CantidadExpress = clientes.Count(c => c.Tipo == TipoCliente.Express);
            reporte.CantidadCarro = clientes.Count(c => c.Tipo == TipoCliente.CarroCompleto);
            // ------------------------------

            if (reporte.TotalClientesAtendidos == 0) return reporte;

            var tiemposEspera = clientes.Select(c => c.ObtenerTiempoEspera()).OrderBy(t => t).ToList();
            var tiemposSistema = clientes.Select(c => c.ObtenerTiempoEnSistema()).ToList();

            reporte.TiempoPromedioEnSistema = tiemposSistema.Average();
            reporte.TiempoPromedioEnCola = tiemposEspera.Average();

            double sumaTotalEspera = tiemposEspera.Sum();
            if (tiempoTotalSimulacion > 0)
            {
                reporte.LargoColaPromedio = sumaTotalEspera / tiempoTotalSimulacion;
            }

            reporte.TiempoMaximoEspera = tiemposEspera.LastOrDefault();
            reporte.LongitudMaximaCola = maxColaRegistrada;

            reporte.Percentil50 = CalcularPercentil(tiemposEspera, 0.50);
            reporte.Percentil90 = CalcularPercentil(tiemposEspera, 0.90);
            reporte.Percentil95 = CalcularPercentil(tiemposEspera, 0.95);

            foreach (var kvp in tiemposCajasAcumulados)
            {
                double utilizacion = 0;
                if (tiempoTotalSimulacion > 0)
                    utilizacion = (kvp.Value / tiempoTotalSimulacion) * 100;

                reporte.UtilizacionPorCaja.Add(kvp.Key, utilizacion);
            }

            return reporte;
        }

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