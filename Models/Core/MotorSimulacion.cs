using System;
using System.Collections.Generic;
using System.Linq;
using CajaExpressSim.Models.Entidades;
using CajaExpressSim.Models.Config;
using CajaExpressSim.Services;

namespace CajaExpressSim.Models.Core
{
    // Tipos de eventos posibles
    public enum TipoEvento
    {
        LlegadaCliente,
        FinAtencion
    }

    // Clase interna para agendar eventos
    public class Evento
    {
        public double Tiempo { get; set; }
        public TipoEvento Tipo { get; set; }
        public Cliente ClienteRelacionado { get; set; } // Para llegadas
        public int IdCajaRelacionada { get; set; }      // Para salidas
    }

    public class MotorSimulacion
    {
        // ==========================================
        // ESTADO DEL SISTEMA
        // ==========================================

        // REFERENCIA AL RELOJ (Objeto inteligente)
        public Reloj Reloj { get; private set; }

        // Atajo opcional para leer el tiempo rápido sin escribir 'Reloj.TiempoActual'
        public double TiempoActual => Reloj.TiempoActual;

        public List<Caja> Cajas { get; private set; }
        public GestorDeColas GestorColas { get; private set; }

        // Lista de Eventos Futuros (FEL)
        private List<Evento> _listaEventos;

        // ==========================================
        // RESULTADOS
        // ==========================================
        public List<Cliente> ClientesAtendidos { get; private set; }

        private int _contadorClientes;

        // Hora de cierre de entrada: 22:00 hs (50400 segundos desde las 08:00)
        private const double TIEMPO_CIERRE = 50400;

        public MotorSimulacion()
        {
            Cajas = new List<Caja>();
            GestorColas = new GestorDeColas();
            _listaEventos = new List<Evento>();
            ClientesAtendidos = new List<Cliente>();

            // Instanciamos el Reloj
            Reloj = new Reloj();
        }

        // ==========================================
        // INICIO
        // ==========================================
        public void Inicializar()
        {
            Reloj.Reiniciar();
            _contadorClientes = 0;

            _listaEventos.Clear();
            GestorColas.Reiniciar();
            ClientesAtendidos.Clear();
            Cajas.Clear();

            // Crear Cajas según configuración del usuario
            for (int i = 0; i < ParametrosGlobales.CantidadCajas; i++)
            {
                Cajas.Add(new Caja(i + 1));
            }

            // Arrancar el motor con el primer evento
            ProgramarProximaLlegada();
        }

        // ==========================================
        // BUCLE PRINCIPAL
        // ==========================================
        public void Simular()
        {
            // Mientras haya eventos pendientes...
            while (_listaEventos.Count > 0)
            {
                // 1. Obtener el evento más próximo
                Evento eventoActual = _listaEventos.OrderBy(e => e.Tiempo).First();
                _listaEventos.Remove(eventoActual);

                // 2. AVANZAR RELOJ usando el método Sincronizar
                Reloj.Sincronizar(eventoActual.Tiempo);

                // 3. Procesar según tipo
                if (eventoActual.Tipo == TipoEvento.LlegadaCliente)
                {
                    ProcesarLlegada(eventoActual);
                }
                else
                {
                    ProcesarSalida(eventoActual);
                }
            }
        }

        // ==========================================
        // LÓGICA DE LLEGADAS
        // ==========================================
        private void ProcesarLlegada(Evento evento)
        {
            // A. Si es antes de las 22:00, programamos la siguiente llegada
            if (Reloj.TiempoActual < TIEMPO_CIERRE)
            {
                ProgramarProximaLlegada();
            }

            // B. Gestionar al cliente que acaba de llegar
            Cliente nuevoCliente = evento.ClienteRelacionado;

            // Buscar caja libre
            Caja cajaLibre = Cajas.FirstOrDefault(c => !c.EstaOcupada);

            if (cajaLibre != null)
            {
                // ATENDER: Pasa directo a caja
                cajaLibre.IniciarAtencion(nuevoCliente, Reloj.TiempoActual);

                // Agendar su salida futura
                var eventoSalida = new Evento
                {
                    Tipo = TipoEvento.FinAtencion,
                    Tiempo = cajaLibre.TiempoFinAtencion,
                    IdCajaRelacionada = cajaLibre.Id
                };
                _listaEventos.Add(eventoSalida);
            }
            else
            {
                // ESPERAR: Va a la cola (el Gestor decide si es VIP o Normal)
                GestorColas.AgregarCliente(nuevoCliente);
            }
        }

        private void ProgramarProximaLlegada()
        {
            // Obtener tasa según la hora actual
            double mediaActual = ObtenerMediaLlegadaSegunHora(Reloj.TiempoActual);

            // Usamos EXPONENCIAL para calcular el intervalo de tiempo
            double tiempoEntreLlegadas = GeneradorEstadistico.Exponencial(mediaActual);

            double proximoTiempo = Reloj.TiempoActual + tiempoEntreLlegadas;

            // Si el próximo cae muy lejos después del cierre y ya estamos cerrados, cortamos.
            if (proximoTiempo > TIEMPO_CIERRE && Reloj.TiempoActual >= TIEMPO_CIERRE)
            {
                return;
            }

            _contadorClientes++;
            var nuevoCliente = new Cliente(_contadorClientes, proximoTiempo);

            var eventoLlegada = new Evento
            {
                Tiempo = proximoTiempo,
                Tipo = TipoEvento.LlegadaCliente,
                ClienteRelacionado = nuevoCliente
            };

            _listaEventos.Add(eventoLlegada);
        }

        // ==========================================
        // LÓGICA DE SALIDAS
        // ==========================================
        private void ProcesarSalida(Evento evento)
        {
            // 1. Identificar caja
            Caja caja = Cajas.First(c => c.Id == evento.IdCajaRelacionada);

            // 2. Finalizar atención y RECUPERAR el cliente
            // (Aquí aplicamos la corrección importante para no perder datos)
            Cliente clienteSaliente = caja.FinalizarAtencion(Reloj.TiempoActual);

            if (clienteSaliente != null)
            {
                ClientesAtendidos.Add(clienteSaliente);
            }

            // 3. Verificar cola para continuar trabajando
            if (!GestorColas.EstaVacia())
            {
                // Sacar siguiente (Prioridad VIP ya resuelta por el Gestor)
                Cliente siguiente = GestorColas.ObtenerSiguienteCliente();

                caja.IniciarAtencion(siguiente, Reloj.TiempoActual);

                var eventoSalida = new Evento
                {
                    Tipo = TipoEvento.FinAtencion,
                    Tiempo = caja.TiempoFinAtencion,
                    IdCajaRelacionada = caja.Id
                };
                _listaEventos.Add(eventoSalida);
            }
        }

        // ==========================================
        // AUXILIARES
        // ==========================================
        private double ObtenerMediaLlegadaSegunHora(double tiempoSegundos)
        {
            if (tiempoSegundos < 14400) // 08:00 a 12:00
            {
                // Ahora usamos la propiedad calculada "Intervalo" que devuelve segundos
                return ParametrosGlobales.IntervaloLlegadaFranja1;
            }
            else if (tiempoSegundos < 25200) // 12:00 a 15:00
            {
                return ParametrosGlobales.IntervaloLlegadaFranja2;
            }
            else // 15:00 a 22:00
            {
                return ParametrosGlobales.IntervaloLlegadaFranja3;
            }
        }
    }
}