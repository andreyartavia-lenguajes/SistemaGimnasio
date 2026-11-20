
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaGimnasio.Data;
using SistemaGimnasio.Models;

namespace SistemaGimnasio.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PlanesMembresiaController : Controller
    {
        private readonly GimnasioContext _context;

        public PlanesMembresiaController(GimnasioContext context)
        {
            _context = context;
        }

        // Listar planes
        public async Task<IActionResult> Index()
        {
            return View(await _context.PlanesMembresia.ToListAsync());
        }

        // Crear plan
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Nombre,Precio,DuracionDias")] PlanesMembresium plan)
        {
            if (plan.Precio <= 0)
            {
                ModelState.AddModelError("Precio", "El precio debe ser mayor a cero.");
            }

            if (plan.DuracionDias <= 0)
            {
                ModelState.AddModelError("DuracionDias", "La duración debe ser mayor a cero.");
            }

            if (ModelState.IsValid)
            {
                _context.PlanesMembresia.Add(plan);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        // Editar plan
        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _context.PlanesMembresia.FindAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("IdPlan,Nombre,Precio,DuracionDias")] PlanesMembresium plan)
        {
            if (id != plan.IdPlan)
            {
                return NotFound();
            }

            if (plan.Precio <= 0)
            {
                ModelState.AddModelError("Precio", "El precio debe ser mayor a cero.");
            }

            if (plan.DuracionDias <= 0)
            {
                ModelState.AddModelError("DuracionDias", "La duración debe ser mayor a cero.");
            }

            if (ModelState.IsValid)
            {
                var planDB = await _context.PlanesMembresia.FindAsync(id);

                if (planDB == null)
                {
                    return NotFound();
                }

                planDB.Nombre = plan.Nombre;
                planDB.Precio = plan.Precio;
                planDB.DuracionDias = plan.DuracionDias;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        // Eliminar plan
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _context.PlanesMembresia.FindAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.PlanesMembresia
                .Include(p => p.Membresia)
                .FirstOrDefaultAsync(p => p.IdPlan == id);

            if (plan == null)
            {
                return NotFound();
            }

            if (plan.Membresia.Any())
            {
                TempData["Error"] = "No se puede eliminar un plan con membresías asociadas.";

                return RedirectToAction(nameof(Index));
            }

            _context.PlanesMembresia.Remove(plan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
