using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGimnasio.Data;
using SistemaGimnasio.Models;

namespace SistemaGimnasio.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class AsistenciasController : Controller
    {
        private readonly GimnasioContext _context;

        public AsistenciasController(GimnasioContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var asistencias = _context.Asistencias
                .Include(a => a.IdClienteNavigation);

            return View(await asistencias.ToListAsync());
        }

        private void CargarListas(int? idCliente = null)
        {
            ViewBag.IdCliente = new SelectList(
                _context.Clientes.Select(c => new
                {
                    c.IdCliente,
                    NombreCompleto = c.Nombre + " " + c.Apellido
                }),
                "IdCliente", "NombreCompleto", idCliente);
        }

        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdCliente,FechaAsistencia")] Asistencia asistencia)
        {
            ModelState.Remove("IdClienteNavigation");

            if (!await _context.Clientes.AnyAsync(
                c => c.IdCliente == asistencia.IdCliente))
            {
                ModelState.AddModelError("IdCliente",
                    "Seleccione un cliente válido.");
            }

            if (asistencia.FechaAsistencia == default)
            {
                ModelState.AddModelError("FechaAsistencia",
                    "Seleccione una fecha válida.");
            }

            if (ModelState.IsValid)
            {
                _context.Asistencias.Add(asistencia);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarListas(asistencia.IdCliente);

            return View(asistencia);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var asistencia = await _context.Asistencias.FindAsync(id);

            if (asistencia == null)
                return NotFound();

            CargarListas(asistencia.IdCliente);

            return View(asistencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("IdAsistencia,IdCliente,FechaAsistencia")] Asistencia asistencia)
        {
            if (id != asistencia.IdAsistencia)
                return NotFound();

            ModelState.Remove("IdClienteNavigation");

            if (!await _context.Clientes.AnyAsync(
                c => c.IdCliente == asistencia.IdCliente))
            {
                ModelState.AddModelError("IdCliente",
                    "Seleccione un cliente válido.");
            }

            if (asistencia.FechaAsistencia == default)
            {
                ModelState.AddModelError("FechaAsistencia",
                    "Seleccione una fecha válida.");
            }

            if (ModelState.IsValid)
            {
                var asistenciaDB = await _context.Asistencias.FindAsync(id);

                if (asistenciaDB == null)
                    return NotFound();

                asistenciaDB.IdCliente = asistencia.IdCliente;
                asistenciaDB.FechaAsistencia = asistencia.FechaAsistencia;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarListas(asistencia.IdCliente);

            return View(asistencia);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var asistencia = await _context.Asistencias
                .Include(a => a.IdClienteNavigation)
                .FirstOrDefaultAsync(a => a.IdAsistencia == id);

            if (asistencia == null)
                return NotFound();

            return View(asistencia);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asistencia = await _context.Asistencias.FindAsync(id);

            if (asistencia == null)
                return NotFound();

            _context.Asistencias.Remove(asistencia);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
