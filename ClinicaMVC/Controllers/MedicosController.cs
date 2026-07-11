using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMVC.Data;
using ClinicaMVC.Models;

namespace ClinicaMVC.Controllers
{
    // Solo el Administrador gestiona el catálogo de médicos
    [Authorize(Roles = "Administrador,Recepcionista")]
    public class MedicosController : Controller
    {
        private readonly ClinicaContext _context;

        public MedicosController(ClinicaContext context)
        {
            _context = context;
        }

        // GET: Medicos (con búsqueda por nombre o especialidad)
        public async Task<IActionResult> Index(string? busqueda)
        {
            var query = _context.Medicos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(m => m.Nombre.Contains(busqueda) || m.Especialidad.Contains(busqueda));
            }

            ViewData["Busqueda"] = busqueda;
            return View(await query.OrderBy(m => m.Nombre).ToListAsync());
        }

        // GET: Medicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.IdMedico == id);
            if (medico == null) return NotFound();

            return View(medico);
        }

        // GET: Medicos/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create() => View();

        // POST: Medicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Nombre,Cedula,Especialidad,Telefono,Turno")] Medico medico)
        {
            if (await _context.Medicos.AnyAsync(m => m.Cedula == medico.Cedula))
            {
                ModelState.AddModelError(nameof(medico.Cedula), "Ya existe un médico registrado con esta cédula.");
            }

            if (!ModelState.IsValid) return View(medico);

            medico.FechaRegistro = DateTime.Now;
            _context.Add(medico);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Médico registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Medicos/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null) return NotFound();

            return View(medico);
        }

        // POST: Medicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdMedico,Nombre,Cedula,Especialidad,Telefono,Turno,FechaRegistro")] Medico medico)
        {
            if (id != medico.IdMedico) return NotFound();

            if (await _context.Medicos.AnyAsync(m => m.Cedula == medico.Cedula && m.IdMedico != id))
            {
                ModelState.AddModelError(nameof(medico.Cedula), "Ya existe otro médico con esta cédula.");
            }

            if (!ModelState.IsValid) return View(medico);

            try
            {
                _context.Update(medico);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Médico actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Medicos.AnyAsync(m => m.IdMedico == id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Medicos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.IdMedico == id);
            if (medico == null) return NotFound();

            return View(medico);
        }

        // POST: Medicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);
            if (medico != null)
            {
                _context.Medicos.Remove(medico);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Médico eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
