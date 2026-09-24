using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicaVeterinaria.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicaVeterinaria.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // REPORTE 1: Listado General de Citas
        // ==========================================
        public async Task<IActionResult> ListadoCitasPDF()
        {
            var citas = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.ServicioVeterinario)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text("Clínica Veterinaria - Listado General de Citas")
                        .SemiBold().FontSize(18).FontColor(Colors.Green.Medium);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Fecha / Hora").Bold();
                            header.Cell().Text("Mascota").Bold();
                            header.Cell().Text("Servicio").Bold();
                            header.Cell().Text("Estado").Bold();
                        });

                        foreach (var c in citas)
                        {
                            table.Cell().Text(c.Fecha.ToString("dd/MM/yyyy HH:mm"));
                            table.Cell().Text(c.Mascota?.Nombre ?? "N/A");
                            table.Cell().Text(c.ServicioVeterinario?.Nombre ?? "N/A");
                            table.Cell().Text(c.Estado);
                        }
                    });

                    page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", "Listado_General_Citas.pdf");
        }

        // ==========================================
        // REPORTE 2: Citas por Usuario / Cliente
        // ==========================================
        public async Task<IActionResult> CitasPorUsuarioPDF(string? usuarioId)
        {
            if (string.IsNullOrEmpty(usuarioId))
            {
                // Si no se recibe un usuario, enviamos el listado a la vista SeleccionarUsuario.cshtml
                ViewBag.Usuarios = new SelectList(_context.Users, "Id", "UserName");
                return View("SeleccionarUsuario");
            }

            var usuario = await _context.Users.FindAsync(usuarioId);
            var citas = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.ServicioVeterinario)
                .Where(c => c.Mascota != null && c.Mascota.UsuarioId == usuarioId)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text($"Historial de Citas - Cliente: {usuario?.UserName ?? "N/A"}")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Fecha / Hora").Bold();
                            header.Cell().Text("Mascota").Bold();
                            header.Cell().Text("Servicio").Bold();
                            header.Cell().Text("Estado").Bold();
                        });

                        foreach (var c in citas)
                        {
                            table.Cell().Text(c.Fecha.ToString("dd/MM/yyyy HH:mm"));
                            table.Cell().Text(c.Mascota?.Nombre ?? "N/A");
                            table.Cell().Text(c.ServicioVeterinario?.Nombre ?? "N/A");
                            table.Cell().Text(c.Estado);
                        }
                    });

                    page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", $"Citas_Cliente_{usuario?.UserName}.pdf");
        }

        // ==========================================
        // REPORTE 3: Servicios Más Solicitados
        // ==========================================
        public async Task<IActionResult> ServiciosMasSolicitadosPDF()
        {
            // Traemos los datos con Include para evaluar el conteo de citas sin error de LINQ a SQL
            var serviciosDb = await _context.ServiciosVeterinarios
                .Include(s => s.Citas)
                .ToListAsync();

            var servicios = serviciosDb
                .Select(s => new
                {
                    s.Nombre,
                    s.Precio,
                    TotalCitas = s.Citas != null ? s.Citas.Count : 0
                })
                .OrderByDescending(s => s.TotalCitas)
                .ToList();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text("Clínica Veterinaria - Servicios Más Solicitados")
                        .SemiBold().FontSize(18).FontColor(Colors.Purple.Medium);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Servicio Veterinario").Bold();
                            header.Cell().Text("Precio ($)").Bold();
                            header.Cell().Text("Total Citas").Bold();
                        });

                        foreach (var s in servicios)
                        {
                            table.Cell().Text(s.Nombre);
                            table.Cell().Text(s.Precio.ToString("C"));
                            table.Cell().Text(s.TotalCitas.ToString());
                        }
                    });

                    page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", "Servicios_Mas_Solicitados.pdf");
        }

        // ==========================================
        // MÉTODOS AUXILIARES (Comprobante y Catálogo)
        // ==========================================
        public async Task<IActionResult> ServiciosPDF()
        {
            var servicios = await _context.ServiciosVeterinarios.ToListAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text("Clínica Veterinaria - Catálogo de Servicios")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(5);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Servicio").Bold();
                            header.Cell().Text("Descripción").Bold();
                            header.Cell().Text("Precio ($)").Bold();
                        });

                        foreach (var s in servicios)
                        {
                            table.Cell().Text(s.Nombre);
                            table.Cell().Text(s.Descripcion);
                            table.Cell().Text(s.Precio.ToString("C"));
                        }
                    });

                    page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", "Catálogo_Servicios.pdf");
        }

        public async Task<IActionResult> ComprobanteCitaPDF(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.ServicioVeterinario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null) return NotFound();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.Header().Text($"Comprobante de Cita #{cita.Id}")
                        .Bold().FontSize(16).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text($"Fecha: {cita.Fecha:dd/MM/yyyy HH:mm}");
                        col.Item().Text($"Mascota: {cita.Mascota?.Nombre}");
                        col.Item().Text($"Servicio: {cita.ServicioVeterinario?.Nombre}");
                        col.Item().Text($"Precio: {cita.ServicioVeterinario?.Precio:C}");
                        col.Item().Text($"Estado: {cita.Estado}");
                    });

                    page.Footer().AlignCenter().Text("¡Gracias por confiar en nuestra clínica!");
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", $"Comprobante_Cita_{id}.pdf");
        }
    }
}