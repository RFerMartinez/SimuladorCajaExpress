namespace CajaExpressSim.Models.Config
{
    public static class ParametrosGlobales
    {
        // ==========================================
        // PARAMETROS DE TIEMPO DE SIMULACIÓN (NUEVO)
        // ==========================================
        public static int SemanasASimular { get; set; } = 1;
        public static int DiasLaboralesPorSemana { get; set; } = 6; // Ej: Lunes a Sábado

        // ==========================================
        // PARAMETROS DE RECURSOS
        // ==========================================
        public static int CantidadCajas { get; set; } = 3;
        public static double TiempoCobroSegundos { get; set; } = 0;

        // ==========================================
        // Capacidad de Cola
        // ==========================================
        public static int CapacidadColaPorCaja { get; set; } = 6;

        // ==========================================
        // TASAS DE LLEGADA
        // ==========================================
        public static double TasaLlegadaFranja1 { get; set; } = 80;
        public static double TasaLlegadaFranja2 { get; set; } = 140;
        public static double TasaLlegadaFranja3 { get; set; } = 100;

        // Propiedades computadas (Segundos)
        public static double IntervaloLlegadaFranja1 => TasaLlegadaFranja1 > 0 ? 3600.0 / TasaLlegadaFranja1 : 0;
        public static double IntervaloLlegadaFranja2 => TasaLlegadaFranja2 > 0 ? 3600.0 / TasaLlegadaFranja2 : 0;
        public static double IntervaloLlegadaFranja3 => TasaLlegadaFranja3 > 0 ? 3600.0 / TasaLlegadaFranja3 : 0;

        // ==========================================
        // PARAMETROS DE SERVICIO
        // ==========================================
        public static double MediaEstandar { get; set; } = 100;
        public static double DesvioEstandar { get; set; } = 20;

        public static double MediaExpress { get; set; } = 70;
        public static double DesvioExpress { get; set; } = 10;

        public static double MediaCarro { get; set; } = 600;
        public static double DesvioCarro { get; set; } = 120;
    }
}