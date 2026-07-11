using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicaMVC.Data;
using ClinicaMVC.Models;

namespace ClinicaMVC.Controllers
{
    // Administrador, Recepcionista y Médico pueden consultar el historial; solo Médico/Administrador lo registran
    [Authorize(Roles = "Administrador,Recepcionista,Medico")]
    public class HistorialMedicoController : Controller
    {
        private readonly ClinicaContext _context;

        public HistorialMedicoController(ClinicaContext context)
        {
            _context = context;
        }

        // GET: HistorialMedico?idPaciente=5  (historial clínico completo de un paciente)
        public async Task<IActionResult> Index(int? idPaciente)
        {
            var query = _context.HistorialesMedicos
                .Include(h => h.Paciente)
                .Include(h => h.Medico)
                .AsQueryable();

            if (idPaciente.HasValue)
            {
                query = query.Where(h => h.IdPaciente == idPaciente.Value);
                ViewBag.Paciente = await _context.Pacientes.FindAsync(idPaciente.Value);
            }

            ViewData["IdPaciente"] = idPaciente;

            return View(await query
                .OrderByDescending(h => h.FechaConsulta)
                .ToListAsync());
        }

        // GET: HistorialMedico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var historial = await _context.HistorialesMedicos
                .Include(h => h.Paciente)
                .Include(h => h.Medico)
                .Include(h => h.Cita)
                .FirstOrDefaultAsync(h => h.IdHistorial == id);
            if (historial == null) return NotFound();

            return View(historial);
        }

        // GET: HistorialMedico/Create (consulta registrada sin una cita previa agendada)
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Create(int? idPaciente)
        {
            await CargarListasAsync(idPaciente);
            return View(new HistorialMedico { IdPaciente = idPaciente ?? 0 });
        }

        // POST: HistorialMedico/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Create([Bind("IdPaciente,IdMedico,Sintomas,Diagnostico,Tratamiento")] HistorialMedico historial)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(historial.IdPaciente, historial.IdMedico);
                return View(historial);
            }

            historial.FechaConsulta = DateTime.Now;
            historial.IdCita = null; // consulta directa, no proviene del flujo "Atender cita"

            _context.Add(historial);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Consulta registrada en el historial médico del paciente.";
            return RedirectToAction(nameof(Index), new { idPaciente = historial.IdPaciente });
        }

        private async Task CargarListasAsync(int? idPacienteSeleccionado = null, int? idMedicoSeleccionado = null)
        {
            ViewBag.Pacientes = new SelectList(
                await _context.Pacientes.OrderBy(p => p.Nombre).ToListAsync(),
                "IdPaciente", "Nombre", idPacienteSeleccionado);

            ViewBag.Medicos = new SelectList(
                await _context.Medicos.OrderBy(m => m.Nombre).ToListAsync(),
                "IdMedico", "Nombre", idMedicoSeleccionado);
        }
    }
}
