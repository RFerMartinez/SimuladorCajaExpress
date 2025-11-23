using System;
using System.Collections.Generic;
using System.Linq;
using CajaExpressSim.Models.Entidades;
using CajaExpressSim.Models.DTOs;

namespace CajaExpressSim.Services
{
    public static class CalculadoraEstadisticas
    {
        public static ReporteSimulacion GenerarReporte(
            List<Cliente> clientes,
            List<Caja> cajas,
            int maxColaRegistrada,
            double tiempoTotalSimulacion)
        {
            var reporte = new ReporteSimulacion();

            // 1. Total Clientes
            reporte.TotalClientesAtendidos = clientes.Count;

            // Validación para evitar división por cero si no se atendió a nadie
            if (reporte.TotalClientesAtendidos == 0)
            {
                return reporte;
            }

            // 2. Promedios (Usamos LINQ para calcular rápido)
            // W = Tiempo en Sistema (Salida - Llegada)
            reporte.TiempoPromedioEnSistema = clientes.Average(c => c.ObtenerTiempoEnSistema());

            // Wq = Tiempo en Cola (InicioAtencion - Llegada)
            reporte.TiempoPromedioEnCola = clientes.Average(c => c.ObtenerTiempoEspera());

            // 3. Máximos (Peor caso)
            reporte.TiempoMaximoEspera = clientes.Max(c => c.ObtenerTiempoEspera());
            reporte.LongitudMaximaCola = maxColaRegistrada; // Dato que viene del GestorDeColas

            // 4. Utilización por Caja
            // Fórmula: (TiempoTotalOcupada / TiempoTotalSimulacion) * 100
            foreach (var caja in cajas)
            {
                double utilizacion = 0;
                if (tiempoTotalSimulacion > 0)
                {
                    utilizacion = (caja.TiempoTotalOcupada / tiempoTotalSimulacion) * 100;
                }

                reporte.UtilizacionPorCaja.Add(caja.Id, utilizacion);
            }

            return reporte;
        }
    }
}