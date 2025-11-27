using System;
using System.IO;
using System.Text;
using CajaExpressSim.Models.DTOs;
using CajaExpressSim.Models.Config; // Para leer qué parámetros se usaron

namespace CajaExpressSim.Services
{
    public static class ExportadorTXT
    {
        public static void GuardarReporte(string rutaArchivo, ReporteSimulacion reporte)
        {
            StringBuilder sb = new StringBuilder();

            // 1. ENCABEZADO
            sb.AppendLine("========================================");
            sb.AppendLine("   REPORTE DE SIMULACIÓN - CAJA EXPRESS");
            sb.AppendLine("========================================");
            sb.AppendLine($"Fecha de generación: {DateTime.Now}");
            sb.AppendLine("----------------------------------------");

            // 2. RESUMEN DE PARÁMETROS (Contexto de la simulación)
            sb.AppendLine("CONFIGURACIÓN APLICADA:");
            sb.AppendLine($"* Cantidad de Cajas: {ParametrosGlobales.CantidadCajas}");
            sb.AppendLine($"* Tiempo de Cobro (Normal): Media {ParametrosGlobales.MediaCobro}s / Desv {ParametrosGlobales.DesvioCobro}s");
            sb.AppendLine($"* Tasas de Llegada (Clientes/Hora): {ParametrosGlobales.TasaLlegadaFranja1} / {ParametrosGlobales.TasaLlegadaFranja2} / {ParametrosGlobales.TasaLlegadaFranja3}");
            sb.AppendLine("----------------------------------------");

            // 3. MÉTRICAS PRINCIPALES
            sb.AppendLine("RESULTADOS DE DESEMPEÑO:");
            sb.AppendLine($"[Total Clientes Atendidos] : {reporte.TotalClientesAtendidos}");
            sb.AppendLine($"[Tiempo Promedio en Sistema (W)] : {reporte.TiempoPromedioEnSistema:F2} seg");
            sb.AppendLine($"[Tiempo Promedio en Cola (Wq)]   : {reporte.TiempoPromedioEnCola:F2} seg");
            sb.AppendLine("");

            sb.AppendLine("INDICADORES CRÍTICOS:");
            sb.AppendLine($"[Cola Máxima Registrada] : {reporte.LongitudMaximaCola} clientes");
            sb.AppendLine($"[Peor Tiempo de Espera]  : {reporte.TiempoMaximoEspera:F2} seg");
            sb.AppendLine("----------------------------------------");

            // 4. DETALLE POR RECURSO
            sb.AppendLine("UTILIZACIÓN DE SERVIDORES:");
            foreach (var kvp in reporte.UtilizacionPorCaja)
            {
                sb.AppendLine($" > Caja #{kvp.Key}: {kvp.Value:F2}% de ocupación");
            }

            sb.AppendLine("========================================");
            sb.AppendLine("FIN DEL REPORTE");

            // 5. GUARDAR ARCHIVO
            // Escribe todo el string en la ruta seleccionada
            File.WriteAllText(rutaArchivo, sb.ToString());
        }
    }
}