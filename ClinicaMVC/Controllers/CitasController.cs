using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicaMVC.Data;
using ClinicaMVC.Models;

namespace ClinicaMVC.Controllers
{
    // Administrador y Recepcionista agendan/gestionan citas; Médico consulta y atiende las suyas
    [Authorize(Roles = "Administrador,Recepcionista,Medico")]
    public class CitasController : Controller
    {
        private readonly ClinicaContext _context;

        public CitasController(ClinicaContext context)
        {
            _context = context;
        }

        // GET: Citas (filtro opcional por fecha y estado)
        public async Task<IActionResult> Index(DateTime? fecha, string? estado)
        {
            var query = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .AsQueryable();

            if (fecha.HasValue)
            {
                query = query.Where(c => c.Fecha.Date == fecha.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(c => c.Estado == estado);
            }

            ViewData["Fecha"] = fecha?.ToString("yyyy-MM-dd");
            ViewData["Estado"] = estado;

            return View(await query
                .OrderBy(c => c.Fecha).ThenBy(c => c.Hora)
                .ToListAsync());
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.IdCita == id);
            if (cita == null) return NotFound();

            return View(cita);
        }

        // GET: Citas/Create
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View();
        }

        // POST: Citas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> Create([Bind("IdPaciente,IdMedico,Fecha,Hora,Observaciones")] Cita cita)
        {
            await ValidarDisponibilidadAsync(cita);

            if (!ModelState.IsValid)
            {
                await CargarListasAsync(cita.IdPaciente, cita.IdMedico);
                return View(cita);
            }

            cita.Estado = EstadosCita.Pendiente;
            _context.Add(cita);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Cita agendada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Citas/Edit/5
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return NotFound();

            await CargarListasAsync(cita.IdPaciente, cita.IdMedico);
            return View(cita);
        }

        // POST: Citas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> Edit(int id, [Bind("IdCita,IdPaciente,IdMedico,Fecha,Hora,Estado,Observaciones")] Cita cita)
        {
            if (id != cita.IdCita) return NotFound();

            await ValidarDisponibilidadAsync(cita);

            if (!ModelState.IsValid)
            {
                await CargarListasAsync(cita.IdPaciente, cita.IdMedico);
                return View(cita);
            }

            try
            {
                _context.Update(cita);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cita actualizada correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Citas.AnyAsync(c => c.IdCita == id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Citas/Cancelar/5 (cambia el estado en vez de eliminar el registro)
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> Cancelar(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.IdCita == id);
            if (cita == null) return NotFound();
            if (cita.Estado != EstadosCita.Pendiente)
            {
                TempData["Mensaje"] = "Solo se pueden cancelar citas en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            return View(cita);
        }

        // POST: Citas/Cancelar/5
        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> CancelarConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null && cita.Estado == EstadosCita.Pendiente)
            {
                cita.Estado = EstadosCita.Cancelada;
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cita cancelada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Citas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.IdCita == id);
            if (cita == null) return NotFound();

            return View(cita);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
            {
                // Si la cita ya generó historial médico, se conserva la integridad (Restrict) y se avisa
                if (await _context.HistorialesMedicos.AnyAsync(h => h.IdCita == id))
                {
                    TempData["Mensaje"] = "No se puede eliminar: la cita tiene un historial médico asociado.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cita eliminada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Citas/Atender/5 (registra el historial médico y marca la cita como Atendida)
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Atender(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.IdCita == id);
            if (cita == null) return NotFound();

            if (cita.Estado != EstadosCita.Pendiente)
            {
                TempData["Mensaje"] = "Solo se pueden atender citas en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            var model = new AtenderCitaViewModel
            {
                IdCita = cita.IdCita,
                NombrePaciente = cita.Paciente?.Nombre ?? string.Empty,
                NombreMedico = cita.Medico?.Nombre ?? string.Empty,
                Fecha = cita.Fecha,
                Hora = cita.Hora
            };

            return View(model);
        }

        // POST: Citas/Atender/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Atender(int id, [Bind("IdCita,Sintomas,Diagnostico,Tratamiento")] AtenderCitaViewModel model)
        {
            if (id != model.IdCita) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.IdCita == id);
            if (cita == null) return NotFound();

            if (cita.Estado != EstadosCita.Pendiente)
            {
                TempData["Mensaje"] = "Solo se pueden atender citas en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.NombrePaciente = cita.Paciente?.Nombre ?? string.Empty;
                model.NombreMedico = cita.Medico?.Nombre ?? string.Empty;
                model.Fecha = cita.Fecha;
                model.Hora = cita.Hora;
                return View(model);
            }

            var historial = new HistorialMedico
            {
                IdPaciente = cita.IdPaciente,
                IdMedico = cita.IdMedico,
                IdCita = cita.IdCita,
                FechaConsulta = DateTime.Now,
                Sintomas = model.Sintomas,
                Diagnostico = model.Diagnostico,
                Tratamiento = model.Tratamiento
            };

            cita.Estado = EstadosCita.Atendida;

            _context.Add(historial);
            _context.Update(cita);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Cita atendida. Se registró el historial médico del paciente.";
            return RedirectToAction("Index", "HistorialMedico", new { idPaciente = cita.IdPaciente });
        }

        // Verifica que el médico no tenga ya otra cita activa (Pendiente/Atendida) en la misma fecha/hora
        private async Task ValidarDisponibilidadAsync(Cita cita)
        {
            if (cita.IdMedico == 0) return;

            bool ocupado = await _context.Citas.AnyAsync(c =>
                c.IdCita != cita.IdCita &&
                c.IdMedico == cita.IdMedico &&
                c.Fecha.Date == cita.Fecha.Date &&
                c.Hora == cita.Hora &&
                c.Estado != EstadosCita.Cancelada);

            if (ocupado)
            {
                ModelState.AddModelError(string.Empty,
                    "El médico ya tiene una cita agendada en esa fecha y hora. Elija otro horario.");
            }
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
