using System;

namespace CajaExpressSim.Models.Entidades
{
    public class Caja
    {
        // ==========================================
        // PROPIEDADES DE ESTADO
        // ==========================================
        public int Id { get; private set; }

        // Indica si está atendiendo a alguien
        public bool EstaOcupada { get; private set; }

        // Referencia al cliente actual (útil para saber a quién liberar)
        public Cliente ClienteActual { get; private set; }

        // Momento exacto en el futuro donde esta caja se liberará
        public double TiempoFinAtencion { get; private set; }

        // ==========================================
        // ACUMULADORES PARA REPORTES (Inciso "Salidas")
        // ==========================================
        public int CantidadClientesAtendidos { get; private set; }

        // Suma de todos los tiempos de servicio. 
        // Vital para calcular: Utilización (%) = (TiempoTotalOcupada / TiempoSimulacion) * 100
        public double TiempoTotalOcupada { get; private set; }

        // Constructor
        public Caja(int id)
        {
            Id = id;
            EstaOcupada = false;
            ClienteActual = null;
            TiempoFinAtencion = 0; // 0.0 indica que no tiene evento de fin pendiente

            // Inicializamos contadores en 0
            CantidadClientesAtendidos = 0;
            TiempoTotalOcupada = 0;
        }

        // ==========================================
        // MÉTODOS DE LÓGICA
        // ==========================================

        /// <summary>
        /// Inicia el servicio de un cliente.
        /// Calcula cuándo terminará basándose en la carga de trabajo del cliente.
        /// </summary>
        public void IniciarAtencion(Cliente cliente, double relojActual)
        {
            if (EstaOcupada)
                throw new InvalidOperationException($"Error Crítico: La Caja {Id} intentó atender a un cliente estando ocupada.");

            EstaOcupada = true;
            ClienteActual = cliente;

            // 1. Marcamos en el cliente a qué hora empezó (para su estadístico de Espera)
            cliente.HoraInicioAtencion = relojActual;

            // 2. Agendamos el fin de servicio.
            // Nota: Usamos cliente.TiempoServicio porque definimos que "la carga la trae el cliente".
            TiempoFinAtencion = relojActual + cliente.TiempoServicio;
        }

        /// <summary>
        /// Finaliza el servicio, actualiza contadores y libera la caja.
        /// </summary>
        public Cliente FinalizarAtencion(double relojActual) // Cambiar void por Cliente
        {
            if (!EstaOcupada || ClienteActual == null) return null;

            TiempoTotalOcupada += ClienteActual.TiempoServicio;
            CantidadClientesAtendidos++;
            ClienteActual.HoraSalida = relojActual;

            Cliente clienteSaliente = ClienteActual; // Guardamos referencia temporal

            ClienteActual = null;
            EstaOcupada = false;
            TiempoFinAtencion = 0;

            return clienteSaliente; // Devolvemos el objeto
        }
    }
}