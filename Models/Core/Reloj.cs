using System;

namespace CajaExpressSim.Models.Core
{
    public class Reloj
    {
        // Propiedad principal: Tiempo absoluto en segundos desde el inicio
        public double TiempoActual { get; private set; }

        // Configuración: La simulación arranca a las 08:00 AM
        private const int HORA_INICIO = 8;

        public Reloj()
        {
            TiempoActual = 0;
        }

        /// <summary>
        /// Avanza el reloj al tiempo del nuevo evento.
        /// </summary>
        public void Sincronizar(double nuevoTiempo)
        {
            // Validación simple para asegurar que el tiempo siempre avanza
            if (nuevoTiempo >= TiempoActual)
            {
                TiempoActual = nuevoTiempo;
            }
        }

        public void Reiniciar()
        {
            TiempoActual = 0;
        }

        /// <summary>
        /// Convierte los segundos simulados en un formato legible "HH:mm:ss".
        /// Útil para mostrar en la interfaz WPF.
        /// </summary>
        public string ObtenerHoraFormateada()
        {
            // 1. Calculamos la hora base inicial en segundos (08:00 = 8 * 3600)
            double segundosInicio = HORA_INICIO * 3600;

            // 2. Sumamos el tiempo transcurrido
            double totalSegundosDia = segundosInicio + TiempoActual;

            // 3. Usamos TimeSpan para formatear
            TimeSpan ts = TimeSpan.FromSeconds(totalSegundosDia);

            // Retorna formato "08:05:12"
            return ts.ToString(@"hh\:mm\:ss");
        }
    }
}