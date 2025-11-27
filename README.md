# 🛒 Caja Express Analytics - Simulador de Eventos Discretos

![Estado del Proyecto](https://img.shields.io/badge/Estado-Finalizado-success)
![Tecnología](https://img.shields.io/badge/C%23-.NET%20Framework%204.7.2-blue)
![Plataforma](https://img.shields.io/badge/Plataforma-Windows%20WPF-blueviolet)

Simulador estocástico de un sistema de colas para un supermercado, para la cátedra de SIMULACIÓN de la carrera Ing. en Sistemas de Información en UNCAUS, desarrollado en C# (WPF) bajo el patrón **MVVM**. Este proyecto implementa un motor de simulación basado en eventos discretos (*Discrete Event Simulation*) para analizar el desempeño y la congestión en líneas de espera con prioridades.

## 📋 Características Principales

* **Motor de Simulación Puro:** Implementación manual de algoritmos de generación de variables aleatorias (Congruencial Mixto, Transformada Inversa, Convolución) sin depender de librerías estadísticas externas.
* **Lógica de Cola Realista:** Modelo de **Cola Única Compartida** con sistema de prioridad preventiva (clientes con 1 artículo tienen prioridad VIP) y capacidad finita con rechazo (*Balking*).
* **Panel de Control (Dashboard):** Interfaz gráfica moderna (Dark Mode) con visualización de KPIs en tiempo real:
    * Tiempo promedio de espera ($W_q$) y en sistema ($W$).
    * Gráfico de evolución de la cola hora por hora.
    * Métricas de calidad de servicio (Percentiles P50, P90, P95).
    * Utilización detallada por recurso (Caja).
* **Reportes:** Exportación de resultados detallados a **PDF** (vía QuestPDF) y **TXT**.

## 🚀 Instalación y Puesta en Marcha

Sigue estos pasos para clonar el proyecto y restaurar las dependencias necesarias.

### 1. Clonar el Repositorio
Abre tu terminal (Git Bash, PowerShell o CMD) y ejecuta:

```bash
git clone https://github.com/RFerMartinez/SimuladorCajaExpress.git
cd CajaExpressSim
```
### 2. Restaurar Dependencias (NuGet)
Este proyecto utiliza librerías externas como **QuestPDF** y **SkiaSharp** para la generación de reportes y gráficos. Es necesario restaurarlas antes de compilar para evitar errores de referencias faltantes.

**Opción A: Automática (Al compilar)**
Simplemente intenta compilar el proyecto (`Ctrl + Shift + B`). Visual Studio debería intentar descargar los paquetes faltantes automáticamente si la opción está habilitada.

**Opción B: Consola del Administrador de Paquetes (Manual)**
Si tienes errores de referencias, sigue estos pasos para forzar la reinstalación:

1.  En Visual Studio, ve al menú: **Herramientas** > **Administrador de paquetes NuGet** > **Consola del Administrador de paquetes**.
2.  En la consola inferior, escribe el siguiente comando y presiona Enter:

```powershell
Update-Package -reinstall
