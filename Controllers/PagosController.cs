
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGimnasio.Data;
using SistemaGimnasio.Models;

namespace SistemaGimnasio.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class PagosController : Controller
    {
        private readonly GimnasioContext _context;

        public PagosController(GimnasioContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pagos = _context.Pagos
                .Include(p => p.IdMembresiaNavigation)
                .ThenInclude(m => m.IdClienteNavigation);

            return View(await pagos.ToListAsync());
        }

        private void CargarListas(int? idMembresia = null)
        {
            ViewBag.IdMembresia = new SelectList(
                _context.Membresias.Select(m => new
                {
                    m.IdMembresia,
                    Nombre = m.IdMembresia + " - " +
                             m.IdClienteNavigation.Nombre + " " +
                             m.IdClienteNavigation.Apellido + " - " +
                             m.IdPlanNavigation.Nombre
                }),
                "IdMembresia", "Nombre", idMembresia);
        }

        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdMembresia,FechaPago,Monto")] Pago pago)
        {
            ModelState.Remove("IdMembresiaNavigation");

            if (!await _context.Membresias.AnyAsync(
                m => m.IdMembresia == pago.IdMembresia))
            {
                ModelState.AddModelError("IdMembresia",
                    "Seleccione una membresía válida.");
            }

            if (pago.Monto <= 0)
            {
                ModelState.AddModelError("Monto",
                    "El monto debe ser mayor a cero.");
            }

            if (pago.FechaPago == default)
            {
                ModelState.AddModelError("FechaPago",
                    "Seleccione una fecha válida.");
            }

            if (ModelState.IsValid)
            {
                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarListas(pago.IdMembresia);
            return View(pago);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);

            if (pago == null)
                return NotFound();

            CargarListas(pago.IdMembresia);

            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("IdPago,IdMembresia,FechaPago,Monto")] Pago pago)
        {
            if (id != pago.IdPago)
                return NotFound();

            ModelState.Remove("IdMembresiaNavigation");

            if (!await _context.Membresias.AnyAsync(
                m => m.IdMembresia == pago.IdMembresia))
            {
                ModelState.AddModelError("IdMembresia",
                    "Seleccione una membresía válida.");
            }

            if (pago.Monto <= 0)
            {
                ModelState.AddModelError("Monto",
                    "El monto debe ser mayor a cero.");
            }

            if (pago.FechaPago == default)
            {
                ModelState.AddModelError("FechaPago",
                    "Seleccione una fecha válida.");
            }

            if (ModelState.IsValid)
            {
                var pagoDB = await _context.Pagos.FindAsync(id);

                if (pagoDB == null)
                    return NotFound();

                pagoDB.IdMembresia = pago.IdMembresia;
                pagoDB.FechaPago = pago.FechaPago;
                pagoDB.Monto = pago.Monto;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarListas(pago.IdMembresia);

            return View(pago);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.IdMembresiaNavigation)
                .ThenInclude(m => m.IdClienteNavigation)
                .FirstOrDefaultAsync(p => p.IdPago == id);

            if (pago == null)
                return NotFound();

            return View(pago);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);

            if (pago == null)
                return NotFound();

            _context.Pagos.Remove(pago);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
