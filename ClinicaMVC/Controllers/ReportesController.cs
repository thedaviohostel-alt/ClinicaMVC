using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMVC.Data;
using ClinicaMVC.Models;
using ClinicaMVC.Services;

namespace ClinicaMVC.Controllers
{
    // Los tres roles consultan reportes; el contenido de "Citas" ya está acotado por fecha, no por rol
    [Authorize(Roles = "Administrador,Recepcionista,Medico")]
    public class ReportesController : Controller
    {
        private readonly ClinicaContext _context;
        private readonly ReporteService _reporteService;

        public ReportesController(ClinicaContext context, ReporteService reporteService)
        {
            _context = context;
            _reporteService = reporteService;
        }

        // GET: Reportes (pantalla principal con los enlaces/formularios de cada reporte)
        public IActionResult Index()
        {
            return View(new ReporteCitasFiltroViewModel());
        }

        // GET: Reportes/PacientesPdf
        public async Task<IActionResult> PacientesPdf()
        {
            var pacientes = await _context.Pacientes.OrderBy(p => p.Nombre).ToListAsync();
            var pdf = _reporteService.GenerarPacientesPdf(pacientes);
            return File(pdf, "application/pdf", $"reporte_pacientes_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        // GET: Reportes/PacientesExcel
        public async Task<IActionResult> PacientesExcel()
        {
            var pacientes = await _context.Pacientes.OrderBy(p => p.Nombre).ToListAsync();
            var excel = _reporteService.GenerarPacientesExcel(pacientes);
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"reporte_pacientes_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        // POST: Reportes/CitasPdf
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CitasPdf(ReporteCitasFiltroViewModel filtro)
        {
            var citas = await ObtenerCitasFiltradasAsync(filtro);
            var pdf = _reporteService.GenerarCitasPdf(citas, filtro.FechaInicio, filtro.FechaFin);
            return File(pdf, "application/pdf", $"reporte_citas_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        // POST: Reportes/CitasExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CitasExcel(ReporteCitasFiltroViewModel filtro)
        {
            var citas = await ObtenerCitasFiltradasAsync(filtro);
            var excel = _reporteService.GenerarCitasExcel(citas, filtro.FechaInicio, filtro.FechaFin);
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"reporte_citas_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        private async Task<List<Cita>> ObtenerCitasFiltradasAsync(ReporteCitasFiltroViewModel filtro)
        {
            var query = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Where(c => c.Fecha.Date >= filtro.FechaInicio.Date && c.Fecha.Date <= filtro.FechaFin.Date);

            if (!string.IsNullOrWhiteSpace(filtro.Estado))
            {
                query = query.Where(c => c.Estado == filtro.Estado);
            }

            return await query.OrderBy(c => c.Fecha).ThenBy(c => c.Hora).ToListAsync();
        }
    }
}
