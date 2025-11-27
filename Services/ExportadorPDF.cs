using System;
using System.IO;
using CajaExpressSim.Models.DTOs;
using CajaExpressSim.Models.Config;
// Usings de QuestPDF
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CajaExpressSim.Services
{
    public static class ExportadorPDF
    {
        public static void GuardarReporte(string rutaArchivo, ReporteSimulacion reporte)
        {
            // ==============================================================
            // IMPORTANTE: Configuración de Licencia Community (Gratuita)
            // Sin esto, la librería lanza una excepción al ejecutar.
            // ==============================================================
            QuestPDF.Settings.License = LicenseType.Community;

            // Generación del Documento usando el patrón Fluent API
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    // 1. Configuración de la Página
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // 2. Encabezado (Header)
                    page.Header()
                        .Text($"Reporte de Simulación - Caja Express")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    // 3. Contenido (Content)
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        // Fecha
                        col.Item().Text($"Fecha de emisión: {DateTime.Now:g}").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingBottom(10); // Espacio

                        // SECCIÓN 1: CONFIGURACIÓN
                        col.Item().Text("1. Parámetros de Configuración").Bold().FontSize(14);
                        col.Item().PaddingBottom(5);

                        col.Item().Table(table =>
                        {
                            // Definir columnas
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Encabezados
                            table.Header(header =>
                            {
                                header.Cell().Element(EstiloCeldaEncabezado).Text("Parámetro");
                                header.Cell().Element(EstiloCeldaEncabezado).Text("Valor Aplicado");
                            });

                            // Filas
                            table.Cell().Element(EstiloCelda).Text("Cantidad de Cajas");
                            table.Cell().Element(EstiloCelda).Text(ParametrosGlobales.CantidadCajas.ToString());

                            table.Cell().Element(EstiloCelda).Text("Tiempo de Cobro (Normal)");
                            table.Cell().Element(EstiloCelda).Text($"Media: {ParametrosGlobales.MediaCobro}s / Desv: {ParametrosGlobales.DesvioCobro}s");

                            table.Cell().Element(EstiloCelda).Text("Tasas de Llegada (Franjas 1/2/3)");
                            // Mostramos la tasa original
                            table.Cell().Element(EstiloCelda).Text($"{ParametrosGlobales.TasaLlegadaFranja1} / {ParametrosGlobales.TasaLlegadaFranja2} / {ParametrosGlobales.TasaLlegadaFranja3} [Clientes/Hora]");
                        });

                        col.Item().PaddingBottom(20); // Espacio grande

                        // SECCIÓN 2: RESULTADOS GENERALES
                        col.Item().Text("2. Métricas de Desempeño (KPIs)").Bold().FontSize(14);
                        col.Item().PaddingBottom(5);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(EstiloCelda).Text("Total Clientes Atendidos");
                            table.Cell().Element(EstiloCelda).Text(reporte.TotalClientesAtendidos.ToString()).Bold();

                            table.Cell().Element(EstiloCelda).Text("Tiempo Prom. en Sistema (W)");
                            table.Cell().Element(EstiloCelda).Text($"{reporte.TiempoPromedioEnSistema:F2} segundos");

                            table.Cell().Element(EstiloCelda).Text("Tiempo Prom. en Cola (Wq)");
                            table.Cell().Element(EstiloCelda).Text($"{reporte.TiempoPromedioEnCola:F2} segundos");

                            table.Cell().Element(EstiloCelda).Text("Tiempo Máximo de Espera");
                            table.Cell().Element(EstiloCelda).Text($"{reporte.TiempoMaximoEspera:F2} segundos").FontColor(Colors.Red.Medium);

                            table.Cell().Element(EstiloCelda).Text("Cola Máxima Registrada");
                            table.Cell().Element(EstiloCelda).Text($"{reporte.LongitudMaximaCola} personas");
                        });

                        col.Item().PaddingBottom(20);

                        // SECCIÓN 3: UTILIZACIÓN
                        col.Item().Text("3. Detalle por Caja").Bold().FontSize(14);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(EstiloCeldaEncabezado).Text("Caja ID");
                                header.Cell().Element(EstiloCeldaEncabezado).Text("Porcentaje de Ocupación");
                            });

                            foreach (var item in reporte.UtilizacionPorCaja)
                            {
                                table.Cell().Element(EstiloCelda).Text($"Caja #{item.Key}");
                                // Barra visual simple (texto)
                                table.Cell().Element(EstiloCelda).Text($"{item.Value:F2} %");
                            }
                        });
                    });

                    // 4. Pie de Página (Footer)
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(rutaArchivo); // Guardar en disco
        }

        // ==========================================
        // Estilos Reutilizables (Para no repetir código)
        // ==========================================
        static IContainer EstiloCelda(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(5);
        }

        static IContainer EstiloCeldaEncabezado(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten3)
                .PaddingVertical(5)
                .PaddingHorizontal(5)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Medium);
        }
    }
}