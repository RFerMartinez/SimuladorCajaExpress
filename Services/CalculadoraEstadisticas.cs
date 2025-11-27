using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
            double tiempoTotalSimulacion,
            int totalRechazados)
        {
            var reporte = new ReporteSimulacion();

            reporte.TotalClientesAtendidos = clientes.Count;
            reporte.CantidadClientesRechazados = totalRechazados;
            reporte.CantidadEstandar = clientes.Count(c => c.Tipo == TipoCliente.Estandar);
            reporte.CantidadExpress = clientes.Count(c => c.Tipo == TipoCliente.Express);
            reporte.CantidadCarro = clientes.Count(c => c.Tipo == TipoCliente.CarroCompleto);

            if (reporte.TotalClientesAtendidos == 0) return reporte;

            var tiemposEspera = clientes.Select(c => c.ObtenerTiempoEspera()).OrderBy(t => t).ToList();
            var tiemposSistema = clientes.Select(c => c.ObtenerTiempoEnSistema()).ToList();

            reporte.TiempoPromedioEnSistema = tiemposSistema.Average();
            reporte.TiempoPromedioEnCola = tiemposEspera.Average();

            double sumaTotalEspera = tiemposEspera.Sum();
            if (tiempoTotalSimulacion > 0)
                reporte.LargoColaPromedio = sumaTotalEspera / tiempoTotalSimulacion;

            reporte.TiempoMaximoEspera = tiemposEspera.LastOrDefault();
            reporte.LongitudMaximaCola = maxColaRegistrada;

            reporte.Percentil50 = CalcularPercentil(tiemposEspera, 0.50);
            reporte.Percentil90 = CalcularPercentil(tiemposEspera, 0.90);
            reporte.Percentil95 = CalcularPercentil(tiemposEspera, 0.95);

            // --- CORRECCIÓN DEL GRÁFICO: DESPLAZAMIENTO HORARIO ---
            reporte.GraficoEsperaPorHora.Clear();
            int horaApertura = 8; // El súper abre a las 8 AM

            // Recorremos las 24 horas reales del día (Eje X del gráfico)
            for (int horaReal = 0; horaReal <= 24; horaReal++)
            {
                double promedioHora = 0;

                // Convertimos Hora Real a Hora de Simulación
                // Ejemplo: Si horaReal es 9 (9 AM), la horaSimulacion es 1.
                int horaSimulacion = horaReal - horaApertura;

                // Solo calculamos si estamos dentro del horario operativo (08:00 en adelante)
                if (horaSimulacion >= 0)
                {
                    double inicioFranja = horaSimulacion * 3600;
                    double finFranja = (horaSimulacion + 1) * 3600;

                    var clientesEnHora = clientes
                        .Where(c => c.HoraLlegada >= inicioFranja && c.HoraLlegada < finFranja)
                        .Select(c => c.ObtenerTiempoEspera())
                        .ToList();

                    if (clientesEnHora.Count > 0)
                    {
                        promedioHora = clientesEnHora.Average();
                    }
                }

                // Agregamos el punto: X = Hora Real (0..24), Y = Espera
                reporte.GraficoEsperaPorHora.Add(new Point(horaReal, promedioHora));
            }
            // -------------------------------------------------------

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