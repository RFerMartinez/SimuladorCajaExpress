using System;
using System.Collections.Generic;
using System.Linq;
using CajaExpressSim.Models.Entidades;
using CajaExpressSim.Models.DTOs;

namespace CajaExpressSim.Services
{
    public static class CalculadoraEstadisticas
    {
        // Método original (para 1 día, opcional mantenerlo)
        public static ReporteSimulacion GenerarReporte(
            List<Cliente> clientes,
            List<Caja> cajas,
            int maxColaRegistrada,
            double tiempoTotalSimulacion)
        {
            // Convertir lista de cajas a diccionario y reusar lógica
            var tiemposDict = cajas.ToDictionary(c => c.Id, c => c.TiempoTotalOcupada);
            return GenerarReporteConsolidado(clientes, tiemposDict, maxColaRegistrada, tiempoTotalSimulacion);
        }

        // --- NUEVO MÉTODO CONSOLIDADO ---
        // Acepta un diccionario con los tiempos ya sumados de todos los días
        public static ReporteSimulacion GenerarReporteConsolidado(
            List<Cliente> clientes,
            Dictionary<int, double> tiemposCajasAcumulados,
            int maxColaRegistrada,
            double tiempoTotalSimulacion)
        {
            var reporte = new ReporteSimulacion();

            reporte.TotalClientesAtendidos = clientes.Count;

            if (reporte.TotalClientesAtendidos == 0) return reporte;

            // Promedios (Promedio de todos los clientes de todas las semanas)
            reporte.TiempoPromedioEnSistema = clientes.Average(c => c.ObtenerTiempoEnSistema());
            reporte.TiempoPromedioEnCola = clientes.Average(c => c.ObtenerTiempoEspera());

            reporte.TiempoMaximoEspera = clientes.Max(c => c.ObtenerTiempoEspera());
            reporte.LongitudMaximaCola = maxColaRegistrada;

            // Cálculo de utilización global
            foreach (var kvp in tiemposCajasAcumulados)
            {
                int idCaja = kvp.Key;
                double tiempoOcupadoTotal = kvp.Value;

                double utilizacion = 0;
                if (tiempoTotalSimulacion > 0)
                {
                    utilizacion = (tiempoOcupadoTotal / tiempoTotalSimulacion) * 100;
                }

                reporte.UtilizacionPorCaja.Add(idCaja, utilizacion);
            }

            return reporte;
        }
    }
}