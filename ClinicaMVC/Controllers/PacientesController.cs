using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMVC.Data;
using ClinicaMVC.Models;

namespace ClinicaMVC.Controllers
{
    // Administrador y Recepcionista gestionan pacientes; Médico solo consulta (ver Details)
    [Authorize(Roles = "Administrador,Recepcionista,Medico")]
    public class PacientesController : Controller
    {
        private readonly ClinicaContext _context;

        public PacientesController(ClinicaContext context)
        {
            _context = context;
        }

        // GET: Pacientes  (con búsqueda por nombre o cédula)
        public async Task<IActionResult> Index(string? busqueda)
        {
            var query = _context.Pacientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(p => p.Nombre.Contains(busqueda) || p.Cedula.Contains(busqueda));
            }

            ViewData["Busqueda"] = busqueda;
            return View(await query.OrderBy(p => p.Nombre).ToListAsync());
        }

        // GET: Pacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdPaciente == id);
            if (paciente == null) return NotFound();

            return View(paciente);
        }

        // GET: Pacientes/Create
        [Authorize(Roles = "Administrador,Recepcionista")]
        public IActionResult Create() => View();

        // POST: Pacientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> Create([Bind("Nombre,Cedula,FechaNacimiento,Telefono,Direccion,TipoSangre")] Paciente paciente)
        {
            if (await _context.Pacientes.AnyAsync(p => p.Cedula == paciente.Cedula))
            {
                ModelState.AddModelError(nameof(paciente.Cedula), "Ya existe un paciente registrado con esta cédula.");
            }

            if (paciente.FechaNacimiento > DateTime.Now)
            {
                ModelState.AddModelError(nameof(paciente.FechaNacimiento), "La fecha de nacimiento no puede ser futura.");
            }

            if (!ModelState.IsValid) return View(paciente);

            paciente.FechaRegistro = DateTime.Now;
            _context.Add(paciente);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Paciente registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Pacientes/Edit/5
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null) return NotFound();

            return View(paciente);
        }

        // POST: Pacientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> Edit(int id, [Bind("IdPaciente,Nombre,Cedula,FechaNacimiento,Telefono,Direccion,TipoSangre,FechaRegistro")] Paciente paciente)
        {
            if (id != paciente.IdPaciente) return NotFound();

            if (await _context.Pacientes.AnyAsync(p => p.Cedula == paciente.Cedula && p.IdPaciente != id))
            {
                ModelState.AddModelError(nameof(paciente.Cedula), "Ya existe otro paciente con esta cédula.");
            }

            if (!ModelState.IsValid) return View(paciente);

            try
            {
                _context.Update(paciente);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Paciente actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Pacientes.AnyAsync(p => p.IdPaciente == id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Pacientes/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdPaciente == id);
            if (paciente == null) return NotFound();

            return View(paciente);
        }

        // POST: Pacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente != null)
            {
                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Paciente eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
