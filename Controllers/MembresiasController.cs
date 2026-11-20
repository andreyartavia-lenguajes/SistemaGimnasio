
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGimnasio.Data;
using SistemaGimnasio.Models;

namespace SistemaGimnasio.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class MembresiasController : Controller
    {
        private readonly GimnasioContext _context;

        public MembresiasController(GimnasioContext context)
        {
            _context = context;
        }

        // Listar membresías
        public async Task<IActionResult> Index()
        {
            var membresias = _context.Membresias
                .Include(m => m.IdClienteNavigation)
                .Include(m => m.IdPlanNavigation);

            return View(await membresias.ToListAsync());
        }

        // Cargar clientes y planes
        private void CargarListas(int? idCliente = null, int? idPlan = null)
        {
            ViewBag.IdCliente = new SelectList(
                _context.Clientes.Select(c => new
                {
                    c.IdCliente,
                    NombreCompleto = c.Nombre + " " + c.Apellido
                }),
                "IdCliente", "NombreCompleto", idCliente);

            ViewBag.IdPlan = new SelectList(
                _context.PlanesMembresia,
                "IdPlan", "Nombre", idPlan);
        }

        // Crear membresía
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdCliente,IdPlan,FechaInicio")] Membresia membresia)
        {
            ModelState.Remove("IdClienteNavigation");
            ModelState.Remove("IdPlanNavigation");

            var plan = await _context.PlanesMembresia
                .FindAsync(membresia.IdPlan);

            if (plan == null)
            {
                ModelState.AddModelError("IdPlan", "Seleccione un plan válido.");
            }

            if (!await _context.Clientes.AnyAsync(
                c => c.IdCliente == membresia.IdCliente))
            {
                ModelState.AddModelError("IdCliente", "Seleccione un cliente válido.");
            }

            if (membresia.FechaInicio == default)
            {
                ModelState.AddModelError("FechaInicio", "Seleccione una fecha válida.");
            }

            if (ModelState.IsValid && plan != null)
            {
                membresia.FechaVencimiento =
                    membresia.FechaInicio.AddDays(plan.DuracionDias);

                _context.Membresias.Add(membresia);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarListas(membresia.IdCliente, membresia.IdPlan);

            return View(membresia);
        }

        // Editar membresía
        public async Task<IActionResult> Edit(int id)
        {
            var membresia = await _context.Membresias.FindAsync(id);

            if (membresia == null)
            {
                return NotFound();
            }

            CargarListas(membresia.IdCliente, membresia.IdPlan);

            return View(membresia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("IdMembresia,IdCliente,IdPlan,FechaInicio")]
            Membresia membresia)
        {
            if (id != membresia.IdMembresia)
            {
                return NotFound();
            }

            ModelState.Remove("IdClienteNavigation");
            ModelState.Remove("IdPlanNavigation");

            var plan = await _context.PlanesMembresia
                .FindAsync(membresia.IdPlan);

            if (plan == null)
            {
                ModelState.AddModelError("IdPlan", "Seleccione un plan válido.");
            }

            if (!await _context.Clientes.AnyAsync(
                c => c.IdCliente == membresia.IdCliente))
            {
                ModelState.AddModelError("IdCliente", "Seleccione un cliente válido.");
            }

            if (membresia.FechaInicio == default)
            {
                ModelState.AddModelError("FechaInicio", "Seleccione una fecha válida.");
            }

            if (ModelState.IsValid && plan != null)
            {
                var membresiaDB = await _context.Membresias.FindAsync(id);

                if (membresiaDB == null)
                {
                    return NotFound();
                }

                membresiaDB.IdCliente = membresia.IdCliente;
                membresiaDB.IdPlan = membresia.IdPlan;
                membresiaDB.FechaInicio = membresia.FechaInicio;
                membresiaDB.FechaVencimiento =
                    membresia.FechaInicio.AddDays(plan.DuracionDias);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarListas(membresia.IdCliente, membresia.IdPlan);

            return View(membresia);
        }

        // Eliminar membresía
        public async Task<IActionResult> Delete(int id)
        {
            var membresia = await _context.Membresias
                .Include(m => m.IdClienteNavigation)
                .Include(m => m.IdPlanNavigation)
                .FirstOrDefaultAsync(m => m.IdMembresia == id);

            if (membresia == null)
            {
                return NotFound();
            }

            return View(membresia);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membresia = await _context.Membresias
                .Include(m => m.Pagos)
                .FirstOrDefaultAsync(m => m.IdMembresia == id);

            if (membresia == null)
            {
                return NotFound();
            }

            if (membresia.Pagos.Any())
            {
                TempData["Error"] = "No se puede eliminar una membresía con pagos asociados.";

                return RedirectToAction(nameof(Index));
            }

            _context.Membresias.Remove(membresia);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
