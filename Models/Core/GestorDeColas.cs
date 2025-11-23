using System;
using System.Collections.Generic;
using System.Linq;
using CajaExpressSim.Models.Entidades;

namespace CajaExpressSim.Models.Core
{
    public class GestorDeColas
    {
        // ==========================================
        // ESTRUCTURAS DE DATOS INTERNAS (Las Dos Colas)
        // ==========================================
        private Queue<Cliente> _colaVip;    // Para clientes con 1 artículo
        private Queue<Cliente> _colaNormal; // Para el resto (Estándar, Carro, Express > 1 art)

        // ==========================================
        // METRICAS INSTANTÁNEAS (Para el Motor y Gráficos)
        // ==========================================
        public int CantidadEnColaVip => _colaVip.Count;
        public int CantidadEnColaNormal => _colaNormal.Count;

        // Total real de gente esperando (Lq actual)
        public int CantidadTotalEnCola => _colaVip.Count + _colaNormal.Count;

        // ==========================================
        // METRICAS HISTÓRICAS (Para el Informe Final)
        // ==========================================
        // Guardamos el pico máximo que alcanzó la cola combinada
        public int MaximaLongitudRegistrada { get; private set; }

        // Constructor
        public GestorDeColas()
        {
            _colaVip = new Queue<Cliente>();
            _colaNormal = new Queue<Cliente>();
            MaximaLongitudRegistrada = 0;
        }

        // ==========================================
        // LÓGICA DE ENCOLADO (Input)
        // ==========================================
        public void AgregarCliente(Cliente cliente)
        {
            // Aquí aplicamos la regla definida en el Modelado:
            // "Si tiene 1 artículo (EsPrioritario), va a la VIP"
            if (cliente.EsPrioritario)
            {
                _colaVip.Enqueue(cliente);
            }
            else
            {
                _colaNormal.Enqueue(cliente);
            }

            // Actualizamos la estadística de "Pico Máximo" si corresponde
            ActualizarEstadisticasMaximas();
        }

        // ==========================================
        // LÓGICA DE DESENCOLADO (Output / Selección)
        // ==========================================
        /// <summary>
        /// Selecciona al siguiente cliente respetando la prioridad.
        /// 1. Mira si hay alguien en VIP.
        /// 2. Si no, mira si hay alguien en Normal.
        /// 3. Si no, retorna null.
        /// </summary>
        public Cliente ObtenerSiguienteCliente()
        {
            if (_colaVip.Count > 0)
            {
                return _colaVip.Dequeue();
            }

            if (_colaNormal.Count > 0)
            {
                return _colaNormal.Dequeue();
            }

            return null; // Nadie esperando
        }

        // ==========================================
        // MÉTODOS AUXILIARES
        // ==========================================
        public bool EstaVacia()
        {
            return CantidadTotalEnCola == 0;
        }

        private void ActualizarEstadisticasMaximas()
        {
            int totalActual = CantidadTotalEnCola;
            if (totalActual > MaximaLongitudRegistrada)
            {
                MaximaLongitudRegistrada = totalActual;
            }
        }

        // Método para limpiar/reiniciar entre corridas de simulación
        public void Reiniciar()
        {
            _colaVip.Clear();
            _colaNormal.Clear();
            MaximaLongitudRegistrada = 0;
        }
    }
}