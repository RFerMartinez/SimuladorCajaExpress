using System.Collections.Generic;

namespace CajaExpressSim.Models.Config
{
    public static class ParametrosGlobales
    {
        // ==========================================
        // PARAMETROS DE RECURSOS Y TIEMPO
        // ==========================================
        public static int CantidadCajas { get; set; } = 3; // Valor por defecto: 3

        // EL NUEVO DATO QUE PEDISTE: Tiempo fijo que demora el cobro (tarjeta/efectivo)
        // Se sumará al tiempo de escaneo de artículos.
        public static double TiempoCobroSegundos { get; set; } = 0;

        // ==========================================
        // PARAMETROS DE LLEGADA (Tasas)
        // ==========================================
        // Medias de tiempo entre llegadas (en segundos) para cada franja
        // Franja 1 (08-12), Franja 2 (12-15), Franja 3 (15-22)
        public static double MediaLlegadaFranja1 { get; set; } = 45.0; // 80 cl/h
        public static double MediaLlegadaFranja2 { get; set; } = 25.7; // 140 cl/h
        public static double MediaLlegadaFranja3 { get; set; } = 36.0; // 100 cl/h

        // ==========================================
        // PARAMETROS DE SERVICIO (Medias y Desvíos)
        // ==========================================
        // Estándar
        public static double MediaEstandar { get; set; } = 100;
        public static double DesvioEstandar { get; set; } = 20;

        // Express
        public static double MediaExpress { get; set; } = 70;
        public static double DesvioExpress { get; set; } = 10;

        // Carro Completo
        public static double MediaCarro { get; set; } = 600;
        public static double DesvioCarro { get; set; } = 120;
    }
}