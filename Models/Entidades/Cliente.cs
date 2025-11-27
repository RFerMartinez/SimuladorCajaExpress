using System;
using CajaExpressSim.Services; // Para usar el GeneradorEstadistico
using CajaExpressSim.Models.Config; // Descomentar cuando creemos ParametrosGlobales

namespace CajaExpressSim.Models.Entidades
{
    // Definimos los tipos posibles según el enunciado
    public enum TipoCliente
    {
        Estandar,
        Express,
        CarroCompleto
    }

    public class Cliente
    {
        // ==========================================
        // ATRIBUTOS DE IDENTIDAD Y ENTRADA
        // ==========================================
        public int Id { get; private set; }
        public double HoraLlegada { get; private set; }
        public TipoCliente Tipo { get; private set; }
        public int CantArticulos { get; private set; }

        // Aquí está el atributo que mencionaste:
        // Se calcula al nacer el cliente, porque depende de SU carga.
        public double TiempoServicio { get; private set; }

        // ==========================================
        // ATRIBUTOS DE LÓGICA (Prioridad)
        // ==========================================
        // Propiedad calculada: Si tiene 1 artículo, es VIP.
        public bool EsPrioritario => CantArticulos == 1;

        // ==========================================
        // ATRIBUTOS DE RESULTADO (Para reportes)
        // ==========================================
        public double HoraInicioAtencion { get; set; } // Se llena cuando entra a Caja
        public double HoraSalida { get; set; }         // Se llena cuando sale de Caja

        // Métodos para calcular métricas finales
        public double ObtenerTiempoEspera()
        {
            return HoraInicioAtencion - HoraLlegada;
        }

        public double ObtenerTiempoEnSistema()
        {
            return HoraSalida - HoraLlegada;
        }

        // ==========================================
        // CONSTRUCTOR
        // ==========================================
        public Cliente(int id, double horaLlegada)
        {
            this.Id = id;
            this.HoraLlegada = horaLlegada;

            // Al crear el cliente, definimos PERFIL y TIEMPO DE SERVICIO automáticamente
            DefinirPerfilAleatorio();
        }

        // ==========================================
        // LÓGICA DE GENERACIÓN DE DATOS
        // ==========================================
        private void DefinirPerfilAleatorio()
        {
            // Variable temporal para el tiempo base de escaneo
            double tiempoEscaneo = 0;
            double u = 0;
            int x = 0;
            double PRA = 0;

            // 1. Iniciar el Ciclo de Aceptación/Rechazo
            do
            {
                // Generar u (Número Aleatorio de Aceptación/Rechazo)
                u = GeneradorEstadistico.GenerarU();

                x = GenerarEnteroUniforme(1, 3);


                switch (x)
                {
                    case 1:
                        PRA = 1.0;
                        break;// Propuesta: Estándar
                    case 2:
                        PRA = 0.30 / 0.60;
                        break;// Propuesta: Express
                    case 3: 
                        PRA = 0.10 / 0.60;
                        break;// Propuesta: Carro Completo
                    default:
                        // No debería ocurrir
                        PRA = 0;
                        break;
                }

            } while (u > PRA); // Repetir mientras u > PRA (Rechazo)

            switch (x)
            {
                case 1: // Tipo: Estándar (60% probabilidad implícita)
                    this.Tipo = TipoCliente.Estandar;
                    // Rango 11-40
                    this.CantArticulos = GenerarEnteroUniforme(11, 40);
                    // Normal(100, 20) - Estos valores deberían venir de ParametrosGlobales
                    tiempoEscaneo = GeneradorEstadistico.Normal(
                        ParametrosGlobales.MediaEstandar,
                        ParametrosGlobales.DesvioEstandar
                    );
                    break;

                case 2: // Tipo: Express (30% probabilidad implícita)
                    this.Tipo = TipoCliente.Express;
                    // Rango 1-10
                    this.CantArticulos = GenerarEnteroUniforme(1, 10);
                    // Normal(70, 10)
                    tiempoEscaneo = GeneradorEstadistico.Normal(
                        ParametrosGlobales.MediaExpress,
                        ParametrosGlobales.DesvioExpress
                    );
                    break;

                case 3: // Tipo: Carro Completo (10% probabilidad implícita)
                    this.Tipo = TipoCliente.CarroCompleto;
                    // Rango 41-100 (Supuesto)
                    this.CantArticulos = GenerarEnteroUniforme(41, 100);
                    // Normal(600, 120)
                    tiempoEscaneo = GeneradorEstadistico.Normal(
                        ParametrosGlobales.MediaCarro,
                        ParametrosGlobales.DesvioCarro
                    );
                    break;

                default:
                    // Este caso no debería ser alcanzado si x está entre 1 y 3
                    throw new InvalidOperationException("Valor X de propuesta de cliente no válido.");
            }

            // CALCULO FINAL DEL TIEMPO DE SERVICIO

            // Generar tiempo de cobro aleatorio (Normal)
            double tiempoCobroAleatorio = GeneradorEstadistico.Normal(
                ParametrosGlobales.MediaCobro,
                ParametrosGlobales.DesvioCobro
            );

            // Protección: El tiempo no puede ser negativo (aunque sea improbable con la Normal)
            if (tiempoCobroAleatorio < 0) tiempoCobroAleatorio = 0;

            // Sumar: Tiempo de Escaneo (Artículos) + Tiempo de Cobro (Pago)
            this.TiempoServicio = tiempoEscaneo + tiempoCobroAleatorio;
        }

        // Helper para generar Uniforme Entera (A + R * (B - A + 1))
        private int GenerarEnteroUniforme(int min, int max)
        {
            double r = GeneradorEstadistico.GenerarU();
            // Fórmula de la transformada inversa para uniforme discreta
            return min + (int)(r * (max - min + 1));
        }
    }
}