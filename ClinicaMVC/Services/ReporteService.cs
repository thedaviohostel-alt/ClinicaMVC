using ClosedXML.Excel;
using ClinicaMVC.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicaMVC.Services
{
    // Centraliza la generación de reportes en PDF y Excel para no repetir
    // la maquetación en cada acción del ReportesController.
    public class ReporteService
    {
        // ---------- PACIENTES ----------

        public byte[] GenerarPacientesPdf(List<Paciente> pacientes)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Sistema de Gestión de Clínica").FontSize(16).Bold();
                        col.Item().Text("Reporte de Pacientes").FontSize(12);
                        col.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CeldaEncabezado).Text("Nombre");
                            header.Cell().Element(CeldaEncabezado).Text("Cédula");
                            header.Cell().Element(CeldaEncabezado).Text("F. Nacimiento");
                            header.Cell().Element(CeldaEncabezado).Text("Teléfono");
                            header.Cell().Element(CeldaEncabezado).Text("Sangre");
                        });

                        foreach (var p in pacientes)
                        {
                            table.Cell().Element(CeldaDato).Text(p.Nombre);
                            table.Cell().Element(CeldaDato).Text(p.Cedula);
                            table.Cell().Element(CeldaDato).Text(p.FechaNacimiento.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CeldaDato).Text(p.Telefono ?? "-");
                            table.Cell().Element(CeldaDato).Text(p.TipoSangre ?? "-");
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return documento.GeneratePdf();
        }

        public byte[] GenerarPacientesExcel(List<Paciente> pacientes)
        {
            using var workbook = new XLWorkbook();
            var hoja = workbook.Worksheets.Add("Pacientes");

            var encabezados = new[] { "Nombre", "Cédula", "Fecha Nacimiento", "Teléfono", "Dirección", "Tipo de Sangre", "Fecha Registro" };
            for (int i = 0; i < encabezados.Length; i++)
            {
                hoja.Cell(1, i + 1).Value = encabezados[i];
                hoja.Cell(1, i + 1).Style.Font.Bold = true;
                hoja.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#0d6efd");
                hoja.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
            }

            int fila = 2;
            foreach (var p in pacientes)
            {
                hoja.Cell(fila, 1).Value = p.Nombre;
                hoja.Cell(fila, 2).Value = p.Cedula;
                hoja.Cell(fila, 3).Value = p.FechaNacimiento;
                hoja.Cell(fila, 3).Style.DateFormat.Format = "dd/MM/yyyy";
                hoja.Cell(fila, 4).Value = p.Telefono ?? "-";
                hoja.Cell(fila, 5).Value = p.Direccion ?? "-";
                hoja.Cell(fila, 6).Value = p.TipoSangre ?? "-";
                hoja.Cell(fila, 7).Value = p.FechaRegistro;
                hoja.Cell(fila, 7).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                fila++;
            }

            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // ---------- CITAS POR RANGO DE FECHAS ----------

        public byte[] GenerarCitasPdf(List<Cita> citas, DateTime desde, DateTime hasta)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Sistema de Gestión de Clínica").FontSize(16).Bold();
                        col.Item().Text("Reporte de Citas Médicas").FontSize(12);
                        col.Item().Text($"Rango: {desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy}").FontSize(9);
                        col.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CeldaEncabezado).Text("Fecha");
                            header.Cell().Element(CeldaEncabezado).Text("Hora");
                            header.Cell().Element(CeldaEncabezado).Text("Paciente");
                            header.Cell().Element(CeldaEncabezado).Text("Médico");
                            header.Cell().Element(CeldaEncabezado).Text("Estado");
                        });

                        foreach (var c in citas)
                        {
                            table.Cell().Element(CeldaDato).Text(c.Fecha.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CeldaDato).Text(c.Hora.ToString("hh\\:mm"));
                            table.Cell().Element(CeldaDato).Text(c.Paciente?.Nombre ?? "-");
                            table.Cell().Element(CeldaDato).Text(c.Medico?.Nombre ?? "-");
                            table.Cell().Element(CeldaDato).Text(c.Estado);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return documento.GeneratePdf();
        }

        public byte[] GenerarCitasExcel(List<Cita> citas, DateTime desde, DateTime hasta)
        {
            using var workbook = new XLWorkbook();
            var hoja = workbook.Worksheets.Add("Citas");

            hoja.Cell(1, 1).Value = $"Reporte de Citas ({desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy})";
            hoja.Range(1, 1, 1, 6).Merge();
            hoja.Cell(1, 1).Style.Font.Bold = true;

            var encabezados = new[] { "Fecha", "Hora", "Paciente", "Médico", "Especialidad", "Estado" };
            for (int i = 0; i < encabezados.Length; i++)
            {
                hoja.Cell(2, i + 1).Value = encabezados[i];
                hoja.Cell(2, i + 1).Style.Font.Bold = true;
                hoja.Cell(2, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#0d6efd");
                hoja.Cell(2, i + 1).Style.Font.FontColor = XLColor.White;
            }

            int fila = 3;
            foreach (var c in citas)
            {
                hoja.Cell(fila, 1).Value = c.Fecha;
                hoja.Cell(fila, 1).Style.DateFormat.Format = "dd/MM/yyyy";
                hoja.Cell(fila, 2).Value = c.Hora.ToString("HH:mm");
                hoja.Cell(fila, 3).Value = c.Paciente?.Nombre ?? "-";
                hoja.Cell(fila, 4).Value = c.Medico?.Nombre ?? "-";
                hoja.Cell(fila, 5).Value = c.Medico?.Especialidad ?? "-";
                hoja.Cell(fila, 6).Value = c.Estado;
                fila++;
            }

            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // ---------- Helpers privados de maquetación PDF (patrón .Element() de QuestPDF) ----------

        private static IContainer CeldaEncabezado(IContainer container)
        {
            return container.Background(Colors.Blue.Medium).Padding(5).DefaultTextStyle(x => x.FontColor(Colors.White).Bold());
        }

        private static IContainer CeldaDato(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
        }
    }
}
