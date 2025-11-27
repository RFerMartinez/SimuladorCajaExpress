using System;

namespace CajaExpressSim.Services
{
    public static class GeneradorEstadistico
    {
        // ==========================================
        // 1. GENERADOR DE NÚMEROS UNIFORMES (0-1)
        // ==========================================

        // Semilla inicial basada en el reloj del sistema
        private static long seed = (long)DateTime.Now.Ticks;

        // Constantes del Generador Congruencial Lineal (LCG)
        private const long a = 1664525;
        private const long m = 2147483647;

        public static Models.DTOs.ReporteSimulacion ReporteSimulacion
        {
            get => default;
            set
            {
            }
        }

        /// <summary>
        /// Genera un número pseudoaleatorio uniforme (u) entre 0 y 1.
        /// </summary>
        public static double GenerarU()
        {
            seed = (a * seed) % m;
            // Es importante asegurar que el resultado sea positivo en caso de desbordamiento, 
            // aunque con 'long' y estos valores estamos seguros, el cast a double hace la división real.
            return (double)seed / m;
        }

        // ==========================================
        // 2. DISTRIBUCIÓN NORMAL (Método Suma de 12)
        // ==========================================

        /// <summary>
        /// Genera una variable aleatoria con distribución Normal.
        /// Basado en el diagrama de flujo: N(media, desviacion).
        /// </summary>
        public static double Normal(double media, double desviacion)
        {
            // 1. Inicio: sum = 0
            double sum = 0;

            // 2. Bucle k=1 hasta 12
            for (int k = 1; k <= 12; k++)
            {
                // Generar u y acumular
                sum += GenerarU();
            }

            // 3. Aplicar fórmula: X = desviacion(Sum - 6) + media
            double X = desviacion * (sum - 6) + media;

            // 4. Fin
            return X;
        }

        // ==========================================
        // 3. DISTRIBUCIÓN POISSON (Método Multiplicativo)
        // ==========================================

        /// <summary>
        /// Genera una variable aleatoria discreta con distribución de Poisson.
        /// Basado en el diagrama de flujo: Poisson(lambda).
        /// </summary>
        public static int Poisson(double lambda)
        {
            // 1. Inicio: prod = 1, X = 0
            double prod = 1;
            int X = 0;

            // Límite e^(-lambda)
            double limite = Math.Exp(-lambda);

            // 2. Condición del bucle: prod >= e^(-lambda)
            while (prod >= limite)
            {
                // Generar u
                double u = GenerarU();

                // prod = prod * u
                prod = prod * u;

                // X = X + 1
                X = X + 1;
            }

            // 3. Retorno (Corrección del algoritmo: X - 1)
            return X - 1;
        }

        // ==========================================
        // 4. DISTRIBUCIÓN EXPONENCIAL (Transformada Inversa)
        // ==========================================

        /// <summary>
        /// Genera una variable aleatoria con distribución Exponencial.
        /// Basado en el diagrama de flujo: Expo(int Z).
        /// Nota: Usamos 'double' para Z (media) para mayor precisión en la simulación.
        /// </summary>
        /// <param name="media">Representa la 'Z' del diagrama (promedio)</param>
        public static double Exponencial(double media)
        {
            // 1. Generar u
            double u = GenerarU();

            // Protección: Math.Log(0) es -Infinito. 
            // Si u es exactamente 0 (muy raro), lo forzamos a un valor muy pequeño.
            if (u == 0) u = 0.00000001;

            // 2. Fórmula: X = -Z * ln(u)
            double X = -media * Math.Log(u);

            // 3. Fin
            return X;
        }
    }
}