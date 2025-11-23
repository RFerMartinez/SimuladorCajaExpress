namespace CajaExpressSim.Models.Config
{
    public static class ParametrosGlobales
    {
        // ==========================================
        // PARAMETROS DE RECURSOS
        // ==========================================
        public static int CantidadCajas { get; set; } = 3;
        public static double TiempoCobroSegundos { get; set; } = 0;

        // ==========================================
        // TASAS DE LLEGADA (NUEVO: Fuente de Verdad = Clientes/Hora)
        // ==========================================
        // Franja 1 (08-12): 80 clientes/h
        public static double TasaLlegadaFranja1 { get; set; } = 80;

        // Franja 2 (12-15): 140 clientes/h
        public static double TasaLlegadaFranja2 { get; set; } = 140;

        // Franja 3 (15-22): 100 clientes/h
        public static double TasaLlegadaFranja3 { get; set; } = 100;

        // ==========================================
        // PROPIEDADES COMPUTADAS (Conversión automática para el Motor)
        // ==========================================
        // El motor necesita "Segundos entre clientes", así que lo calculamos aquí mismo.
        // Fórmula: 3600 / Tasa

        public static double IntervaloLlegadaFranja1 => TasaLlegadaFranja1 > 0 ? 3600.0 / TasaLlegadaFranja1 : 0;
        public static double IntervaloLlegadaFranja2 => TasaLlegadaFranja2 > 0 ? 3600.0 / TasaLlegadaFranja2 : 0;
        public static double IntervaloLlegadaFranja3 => TasaLlegadaFranja3 > 0 ? 3600.0 / TasaLlegadaFranja3 : 0;

        // ==========================================
        // PARAMETROS DE SERVICIO
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